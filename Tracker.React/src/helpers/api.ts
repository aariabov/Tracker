import { userStore } from "../auth/UserStore";
import { BACKEND_URL } from "../config";

export async function get<T>(path: string): Promise<T> {
  try {
    const response = await fetch(BACKEND_URL + path, {
      headers: {
        Authorization: "Bearer " + userStore.token,
      },
    });

    if (!response.ok) {
      throw new Error(response.statusText);
    }

    const data = await response.json();
    return data;
  } catch (err) {
    userStore.logout();
    return Promise.reject();
  }
}

export async function post<TBody, TRes>(
  path: string,
  bodyObj: TBody
): Promise<TRes> {
  try {
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

    if (!response.ok) {
      throw new Error(response.statusText);
    }

    const data = await response.json();
    return data;
  } catch (err) {
    userStore.logout();
    return Promise.reject();
  }
}
