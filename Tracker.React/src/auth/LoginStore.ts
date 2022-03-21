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
  private _error: string | undefined = undefined;

  public get email(): string {
    return this._email;
  }
  public get password(): string {
    return this._password;
  }
  public get error(): string | undefined {
    return this._error;
  }
  public get hasError(): boolean {
    return this._error !== undefined;
  }

  constructor() {
    makeObservable<LoginStore, "_email" | "_password" | "_error">(this, {
      _email: observable,
      _password: observable,
      _error: observable,
      setEmail: action,
      setPassword: action,
      clearError: action,
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

  clearError = (): void => {
    this._error = undefined;
  };

  handleSubmit = async (): Promise<void> => {
    const body: LoginBody = {
      email: this._email,
      password: this._password,
    };

    try {
      const token = await post<LoginBody, string>("api/user/login", body);
      userStore.setToken(token);
      this.clear();
    } catch (err) {
      this._error = "Неправильный email или пароль";
      throw err;
    }
  };
}
