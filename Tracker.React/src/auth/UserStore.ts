import { action, computed, makeObservable, observable } from "mobx";
import { post } from "../helpers/api";

interface UserInfo {
  token: string;
  email: string;
}

interface LoginBody {
  email: string;
  password: string;
}

export class UserStore {
  private _token: string = "";
  private _email: string = "";

  constructor() {
    makeObservable<UserStore, "_email" | "_token">(this, {
      _email: observable,
      _token: observable,
      token: computed,
      email: computed,
      loadUserInfo: action,
    });

    const userInfoStr = localStorage.getItem("userInfo");
    if (userInfoStr) {
      const userInfo: UserInfo = JSON.parse(userInfoStr);
      this._email = userInfo.email;
      this._token = userInfo.token;
    }
  }

  get token(): string {
    return this._token;
  }

  get email(): string {
    return this._email;
  }

  loadUserInfo = async (email: string, password: string): Promise<void> => {
    const body: LoginBody = {
      email,
      password,
    };

    const userInfo = await post<LoginBody, UserInfo>("api/user/login", body);
    localStorage.setItem("userInfo", JSON.stringify(userInfo));
    this._email = userInfo.email;
    this._token = userInfo.token;
  };

  logout = (): void => {
    localStorage.removeItem("userInfo");
    this._email = "";
    this._token = "";
  };
}

export const userStore = new UserStore();
