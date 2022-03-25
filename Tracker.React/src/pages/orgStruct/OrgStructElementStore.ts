import { action, makeObservable, observable } from "mobx";
import { ModelErrors, post } from "../../helpers/api";

export class OrgStructElementStore {
  private _id: number = 0;
  private _name: string = "";
  private _email: string = "";
  private _password: string = "";
  private _parentId: number | undefined = undefined;

  private _errors: Errors | undefined = undefined;

  private _isModalVisible: boolean = false;

  public get name(): string {
    return this._name;
  }
  public get email(): string {
    return this._email;
  }
  public get password(): string {
    return this._password;
  }
  public get parentId(): number | undefined {
    return this._parentId;
  }
  public get errors(): Errors | undefined {
    return this._errors;
  }
  public get isModalVisible(): boolean {
    return this._isModalVisible;
  }

  constructor() {
    makeObservable<
      OrgStructElementStore,
      | "_name"
      | "_email"
      | "_password"
      | "_parentId"
      | "_isModalVisible"
      | "_errors"
    >(this, {
      _name: observable,
      _parentId: observable,
      _email: observable,
      _password: observable,
      _errors: observable,
      _isModalVisible: observable,
      setName: action,
      setParentId: action,
      save: action,
    });
  }

  clear = (): void => {
    this._id = 0;
    this._name = "";
    this._email = "";
    this._password = "";
    this._parentId = undefined;
    this._errors = undefined;
  };

  setName = (value: string): void => {
    this._name = value;
    if (this._errors?.name) this._errors.name = undefined;
  };

  setEmail = (value: string): void => {
    this._email = value;
    if (this._errors?.email) this._errors.email = undefined;
  };

  setPassword = (value: string): void => {
    this._password = value;
    if (this._errors?.password) this._errors.password = undefined;
  };

  setParentId = (value: number): void => {
    this._parentId = value;
    if (this._errors?.bossId) this._errors.bossId = undefined;
  };

  showModal = (): void => {
    this._isModalVisible = true;
    if (this._errors?.name) this._errors.name = undefined;
  };

  hideModal = (): void => {
    this._isModalVisible = false;
    this.clear();
  };

  save = async (): Promise<boolean> => {
    const body: Body = {
      name: this._name,
      email: this._email,
      password: this._password,
      bossId: this._parentId,
    };

    const result = await post<Body, RequestResult>("api/User/register", body);
    if (result.modelErrors) {
      this._errors = result.modelErrors;
      return false;
    } else {
      this.hideModal();
      this.clear();
      return true;
    }
  };
}

type RequestResult = string & ModelErrors<Errors>;

interface Body {
  name: string;
  email: string;
  password: string;
  bossId: number | undefined;
}

interface Errors {
  name?: string;
  email?: string;
  password?: string;
  bossId?: string;
}
