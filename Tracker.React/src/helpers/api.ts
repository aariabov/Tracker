import {
  HttpResponse,
  ProblemDetails,
  RequestParams,
  TokensVm,
} from "../api/Api";
import { apiClient } from "../ApiClient";
import { userStore } from "../auth/UserStore";
import { errorStore } from "../stores/ErrorStore";

export interface ModelErrors<T> {
  modelErrors?: T;
}

let fetchPromise: Promise<HttpResponse<TokensVm, ProblemDetails>> | null;

async function refreshToken(): Promise<void> {
  if (!fetchPromise) {
    fetchPromise = apiClient.api.usersRefreshToken(userStore.refreshToken);
  }

  const response = await fetchPromise;
  fetchPromise = null;
  if (!response.ok) {
    userStore.logout();
    throw new Error(response.statusText);
  }

  userStore.setTokens(response.data.token, response.data.refreshToken);
}

export async function api<T, E>(
  fn: (par: RequestParams) => Promise<HttpResponse<T, E>>
): Promise<HttpResponse<T, E>>;
export async function api<T, E, P>(
  fn: (body: P, par: RequestParams) => Promise<HttpResponse<T, E>>,
  body: P
): Promise<HttpResponse<T, E>>;
export async function api<T, E, P>(
  fn: any,
  body?: P
): Promise<HttpResponse<T, E>> {
  const params: RequestParams = { secure: true };

  try {
    return await (body ? fn(body, params) : fn(params));
  } catch (err: unknown) {
    let response = err as HttpResponse<T, E>;
    if (response.status === 401) {
      await refreshToken();
    } else {
      if (typeof err === "string") {
        errorStore.setError(err);
      } else if (err instanceof Error) {
        errorStore.setError(err.message);
      } else {
        errorStore.setError("unknown error");
      }
      throw err;
    }

    return await (body ? fn(body, params) : fn(params));
  }
}
