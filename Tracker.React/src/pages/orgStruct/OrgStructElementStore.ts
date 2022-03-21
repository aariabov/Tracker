import { action, makeObservable, observable } from "mobx";
import { post } from "../../helpers/api";
import { OrgStructElement } from "../../stores/OrgStructStore";

export class OrgStructElementStore {
  private _id: number = 0;
  private _name: string = "";
  private _email: string = "";
  private _password: string = "";
  private _parentId: number | undefined = undefined;

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
  public get isModalVisible(): boolean {
    return this._isModalVisible;
  }

  constructor() {
    makeObservable<
      OrgStructElementStore,
      "_name" | "_email" | "_password" | "_parentId" | "_isModalVisible"
    >(this, {
      _name: observable,
      _parentId: observable,
      _email: observable,
      _password: observable,
      _isModalVisible: observable,
      setName: action,
      setParentId: action,
    });
  }

  clear = (): void => {
    this._id = 0;
    this._name = "";
    this._email = "";
    this._password = "";
    this._parentId = undefined;
  };

  setName = (value: string): void => {
    this._name = value;
  };

  setEmail = (value: string): void => {
    this._email = value;
  };

  setPassword = (value: string): void => {
    this._password = value;
  };

  setParentId = (value: number): void => {
    this._parentId = value;
  };

  showModal = (): void => {
    this._isModalVisible = true;
  };

  hideModal = (): void => {
    this._isModalVisible = false;
    this.clear();
  };

  save = async (): Promise<void> => {
    const body: Body = {
      name: this._name,
      email: this._email,
      password: this._password,
      bossId: this._parentId,
    };

    const userId = await post<Body, string>("api/User/register", body);

    if (userId) {
      this.hideModal();
      this.clear();
    }
  };
}

interface Body {
  name: string;
  email: string;
  password: string;
  bossId: number | undefined;
}
