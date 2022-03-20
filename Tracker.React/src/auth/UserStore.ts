import { action, computed, makeObservable, observable } from "mobx";

interface UserInfo {
  nameid: string;
  email: string;
}

function parseJwt(token: string): UserInfo | null {
  try {
    return JSON.parse(atob(token.split(".")[1]));
  } catch (e) {
    return null;
  }
}

export class UserStore {
  private _id: string = "";
  private _token: string = "";
  private _email: string = "";

  constructor() {
    makeObservable<UserStore, "_email" | "_token">(this, {
      _email: observable,
      _token: observable,
      token: computed,
      email: computed,
      setToken: action,
    });

    const token = localStorage.getItem("token");
    this.setToken(token);
  }

  get id(): string {
    return this._id;
  }

  get token(): string {
    return this._token;
  }

  get email(): string {
    return this._email;
  }

  setToken = (token: string | null): void => {
    if (token) {
      const userInfo = parseJwt(token);
      if (userInfo) {
        this._id = userInfo.nameid;
        this._email = userInfo.email;
        this._token = token;
        localStorage.setItem("token", token);
      }
    }
  };

  logout = (): void => {
    localStorage.removeItem("token");
    this._email = "";
    this._token = "";
  };
}

export const userStore = new UserStore();
