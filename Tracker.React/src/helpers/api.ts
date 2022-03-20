import { userStore } from "../auth/UserStore";
import { BACKEND_URL } from "../config";
import { errorStore } from "../stores/ErrorStore";

interface BackendError {
  type: string;
  title: string;
  status: number;
  detail: string;
  traceId: string;
}

function isJsonResponse(response: Response): boolean {
  const contentType = response.headers.get("content-type");
  return contentType !== null && contentType.includes("application/json");
}

async function processResponse<T>(response: Response): Promise<T> {
  try {
    if (!response.ok) {
      if (response.status === 401) {
        userStore.logout();
        throw new Error(response.statusText);
      }

      const backendError: BackendError = await response.json();
      throw new Error(backendError.title);
    }

    const data = isJsonResponse(response) ? await response.json() : undefined;
    return data;
  } catch (err: unknown) {
    if (typeof err === "string") {
      errorStore.setError(err);
    } else if (err instanceof Error) {
      errorStore.setError(err.message);
    } else {
      errorStore.setError("unknown error");
    }
    throw err;
  }
}

export async function get<T>(path: string): Promise<T> {
  const response = await fetch(BACKEND_URL + path, {
    headers: {
      Authorization: "Bearer " + userStore.token,
    },
  });

  return processResponse(response);
}

export async function post<TBody, TRes = void>(
  path: string,
  bodyObj: TBody
): Promise<TRes> {
  const body = JSON.stringify(bodyObj);

  const response = await fetch(BACKEND_URL + path, {
    method: "POST",
    credentials: "same-origin",
    headers: {
      Authorization: "Bearer " + userStore.token,
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    body,
  });

  return processResponse(response);
}
