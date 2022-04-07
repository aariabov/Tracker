import { action, computed, makeObservable, observable } from "mobx";

interface UserInfo {
  nameid: string;
  email: string;
  isUserBoss: string;
  role?: string[];
}

function parseJwt(token: string): UserInfo | null {
  try {
    return JSON.parse(atob(token.split(".")[1]));
  } catch (e) {
    return null;
  }
}

const tokenKey = "token";
const refreshTokenKey = "refreshToken";

export class UserStore {
  private _id: string = "";
  private _token: string = "";
  private _refreshToken: string = "";
  private _email: string = "";
  private _roles: string[] = [];
  private _isUserBoss: boolean = false;

  constructor() {
    makeObservable<
      UserStore,
      "_email" | "_token" | "_refreshToken" | "_isUserBoss" | "_roles"
    >(this, {
      _email: observable,
      _token: observable,
      _refreshToken: observable,
      _isUserBoss: observable,
      _roles: observable,
      token: computed,
      email: computed,
      setTokens: action,
    });

    const token = localStorage.getItem(tokenKey);
    const refreshToken = localStorage.getItem(refreshTokenKey);
    if (token && refreshToken) this.setTokens(token, refreshToken);
  }

  get id(): string {
    return this._id;
  }

  get token(): string {
    return this._token;
  }

  get refreshToken(): string {
    return this._refreshToken;
  }

  get email(): string {
    return this._email;
  }

  get isAdmin(): boolean {
    return this._roles.includes("Admin");
  }

  get isAnalyst(): boolean {
    return this._roles.includes("Analyst") || this.isAdmin;
  }

  get isUserBoss(): boolean {
    return this._isUserBoss;
  }

  setTokens = (token: string, refreshToken: string): void => {
    const userInfo = parseJwt(token);
    if (userInfo) {
      this._id = userInfo.nameid;
      this._email = userInfo.email;
      this._isUserBoss = userInfo.isUserBoss === "true";
      this._roles = userInfo.role || [];
      this._token = token;
      localStorage.setItem(tokenKey, token);
    }

    this._refreshToken = refreshToken;
    localStorage.setItem(refreshTokenKey, refreshToken);
  };

  logout = (): void => {
    localStorage.removeItem(tokenKey);
    localStorage.removeItem(refreshTokenKey);
    this._email = "";
    this._token = "";
    this._isUserBoss = false;
  };
}

export const userStore = new UserStore();
