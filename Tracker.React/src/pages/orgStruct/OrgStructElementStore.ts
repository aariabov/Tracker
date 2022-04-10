import { action, makeObservable, observable } from "mobx";
import { ModelErrors, post } from "../../helpers/api";
import { OrgStructElementRow } from "../../stores/OrgStructStore";

export class OrgStructElementStore {
  private _id: string = "";
  private _name: string = "";
  private _email: string = "";
  private _password: string = "";
  private _parentId: string | undefined = undefined;
  private _roles: string[] = [];

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
  public get parentId(): string | undefined {
    return this._parentId;
  }
  public get roles(): string[] {
    return this._roles;
  }
  public get errors(): Errors | undefined {
    return this._errors;
  }
  public get isModalVisible(): boolean {
    return this._isModalVisible;
  }
  public get isEditMode(): boolean {
    return this._id ? true : false;
  }

  constructor() {
    makeObservable<
      OrgStructElementStore,
      | "_name"
      | "_email"
      | "_password"
      | "_parentId"
      | "_roles"
      | "_isModalVisible"
      | "_errors"
    >(this, {
      _name: observable,
      _parentId: observable,
      _email: observable,
      _password: observable,
      _roles: observable,
      _errors: observable,
      _isModalVisible: observable,
      setName: action,
      setParentId: action,
      save: action,
    });
  }

  clear = (): void => {
    this._id = "";
    this._name = "";
    this._email = "";
    this._password = "";
    this._parentId = undefined;
    this._roles = [];
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

  setParentId = (value: string): void => {
    this._parentId = value;
    if (this._errors?.bossId) this._errors.bossId = undefined;
  };

  setRoles = (values: string[]): void => {
    this._roles = values;
    if (this._errors?.roles) this._errors.roles = undefined;
  };

  showModal = (user?: OrgStructElementRow): void => {
    if (user) {
      this._id = user.id;
      this._name = user.name;
      this._email = user.email;
      this._parentId = user.parentId;
      this._roles = user.roles;
    }

    if (this._errors?.name) this._errors.name = undefined;
    this._isModalVisible = true;
  };

  hideModal = (): void => {
    this._isModalVisible = false;
    this.clear();
  };

  deleteUser = async (
    roleId: string
  ): Promise<undefined | ModelErrors<Errors>> => {
    const body: DeletingBody = {
      id: roleId,
    };

    const result = await post<DeletingBody, ModelErrors<Errors>>(
      "api/users/delete",
      body
    );

    return result;
  };

  save = async (): Promise<boolean> => {
    const result = this.isEditMode
      ? await this.updateUser()
      : await this.createUser();

    if (result?.modelErrors) {
      this._errors = result.modelErrors;
      return false;
    } else {
      this.hideModal();
      this.clear();
      return true;
    }
  };

  private async createUser(): Promise<ModelErrors<Errors>> {
    const body: CreationBody = {
      name: this._name,
      email: this._email,
      password: this._password,
      bossId: this._parentId,
      roles: this._roles,
    };

    const result = await post<CreationBody, ModelErrors<Errors>>(
      "api/users/register",
      body
    );

    return result;
  }

  private async updateUser(): Promise<ModelErrors<Errors>> {
    const body: UpdatingBody = {
      id: this._id!,
      name: this._name,
      email: this._email,
      bossId: this._parentId,
      roles: this._roles,
    };

    const result = await post<UpdatingBody, ModelErrors<Errors>>(
      "api/users/update",
      body
    );

    return result;
  }
}

interface CreationBody {
  name: string;
  email: string;
  password: string;
  bossId: string | undefined;
  roles: string[];
}

interface UpdatingBody {
  id: string;
  name: string;
  email: string;
  bossId: string | undefined;
  roles: string[];
}

interface Errors {
  id?: string;
  name?: string;
  email?: string;
  password?: string;
  bossId?: string;
  roles?: string;
}

interface DeletingBody {
  id: string;
}
