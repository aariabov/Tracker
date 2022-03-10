import { action, makeObservable, observable } from "mobx";
import { userStore } from "./UserStore";

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

  handleSubmit = async (e: React.FormEvent): Promise<void> => {
    userStore.loadUserInfo(this._email, this._password);
    this.clear();
  };
}
