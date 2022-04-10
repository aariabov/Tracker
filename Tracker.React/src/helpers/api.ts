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

export interface TokenResponse {
  token: string;
  refreshToken: string;
}

export interface ModelErrors<T> {
  modelErrors?: T;
}

function isJsonResponse(response: Response): boolean {
  const contentType = response.headers.get("content-type");
  return contentType !== null && contentType.includes("application/json");
}

function createPostRequestInit<T>(bodyObj: T): RequestInit {
  const body = JSON.stringify(bodyObj);

  const requestInit: RequestInit = {
    method: "POST",
    headers: {
      Authorization: "Bearer " + userStore.token,
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    body,
  };

  return requestInit;
}

async function parseResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let errorMsg = response.statusText;
    if (isJsonResponse(response)) {
      const backendError: BackendError = await response.json();
      errorMsg = backendError.title;
    }

    throw new Error(errorMsg);
  }

  return isJsonResponse(response) ? response.json() : undefined;
}

let fetchPromise: Promise<Response> | null;
let jsonPromise: Promise<TokenResponse> | null;

async function refreshToken(): Promise<void> {
  if (!fetchPromise) {
    const requestInit = createPostRequestInit(userStore.refreshToken);
    fetchPromise = fetch("api/users/refresh-token", requestInit);
  }

  const response = await fetchPromise;
  fetchPromise = null;
  if (!response.ok) {
    userStore.logout();
    throw new Error(response.statusText);
  }

  if (!jsonPromise) jsonPromise = parseResponse<TokenResponse>(response);

  const data = await jsonPromise;
  jsonPromise = null;
  userStore.setTokens(data.token, data.refreshToken);
}

async function makeRequest<T>(
  url: string,
  requestInit: RequestInit
): Promise<T> {
  try {
    let response = await fetch(url, requestInit);
    if (!response.ok) {
      if (response.status === 401) {
        await refreshToken();
        requestInit.headers = {
          ...requestInit.headers,
          Authorization: "Bearer " + userStore.token,
        };
        response = await fetch(url, requestInit);
      }
    }

    return await parseResponse<T>(response);
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
  const requestInit: RequestInit = {
    headers: {
      Authorization: "Bearer " + userStore.token,
    },
  };

  return makeRequest<T>(BACKEND_URL + path, requestInit);
}

export async function post<TBody, TRes = void>(
  path: string,
  bodyObj: TBody
): Promise<TRes> {
  const requestInit = createPostRequestInit(bodyObj);
  return makeRequest<TRes>(BACKEND_URL + path, requestInit);
}
