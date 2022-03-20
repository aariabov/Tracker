import { action, makeObservable, observable } from "mobx";
import { post } from "../helpers/api";
import { userStore } from "./UserStore";

interface LoginBody {
  email: string;
  password: string;
}

export class LoginStore {
  private _email: string = "";
  private _password: string = "";

  public get email(): string {
    return this._email;
  }
  public get password(): string {
    return this._password;
  }

  constructor() {
    makeObservable<LoginStore, "_email" | "_password">(this, {
      _email: observable,
      _password: observable,
      setEmail: action,
      setPassword: action,
      handleSubmit: action,
    });
  }

  private clear = (): void => {
    this._email = "";
    this._password = "";
  };

  setEmail = (e: React.ChangeEvent<HTMLInputElement>): void => {
    this._email = e.target.value;
  };

  setPassword = (e: React.ChangeEvent<HTMLInputElement>): void => {
    this._password = e.target.value;
  };

  handleSubmit = async (): Promise<void> => {
    const body: LoginBody = {
      email: this._email,
      password: this._password,
    };

    const token = await post<LoginBody, string>("api/user/login", body);
    userStore.setToken(token);
    this.clear();
  };
}
