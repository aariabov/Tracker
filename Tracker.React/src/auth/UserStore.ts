import { action, computed, makeObservable, observable } from "mobx";

interface UserInfo {
  nameid: string;
  email: string;
  isUserBoss: string;
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
  private _isUserBoss: boolean = false;

  constructor() {
    makeObservable<UserStore, "_email" | "_token" | "_isUserBoss">(this, {
      _email: observable,
      _token: observable,
      _isUserBoss: observable,
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

  get isUserBoss(): boolean {
    return this._isUserBoss;
  }

  setToken = (token: string | null): void => {
    if (token) {
      const userInfo = parseJwt(token);
      if (userInfo) {
        this._id = userInfo.nameid;
        this._email = userInfo.email;
        this._isUserBoss = userInfo.isUserBoss === "true";
        this._token = token;
        localStorage.setItem("token", token);
      }
    }
  };

  logout = (): void => {
    localStorage.removeItem("token");
    this._email = "";
    this._token = "";
    this._isUserBoss = false;
  };
}

export const userStore = new UserStore();
