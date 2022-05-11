import { action, makeObservable, observable } from "mobx";
import { ModelErrors, post } from "../../helpers/api";
import { Role } from "./RolesStore";

export class RoleStore {
  private _id: string | undefined = undefined;
  private _name: string = "";
  private _concurrencyStamp: string = "";

  private _errors: Errors | undefined = undefined;

  private _isModalVisible: boolean = false;

  public get name(): string {
    return this._name;
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
    makeObservable<RoleStore, "_id" | "_name" | "_isModalVisible" | "_errors">(
      this,
      {
        _id: observable,
        _name: observable,
        _errors: observable,
        _isModalVisible: observable,
        setName: action,
        save: action,
      }
    );
  }

  clear = (): void => {
    this._id = undefined;
    this._name = "";
    this._errors = undefined;
  };

  setName = (value: string): void => {
    this._name = value;
    if (this._errors?.name) this._errors.name = undefined;
  };

  showModal = (role?: Role): void => {
    if (role) {
      this._id = role.id;
      this._name = role.name;
      this._concurrencyStamp = role.concurrencyStamp;
    }

    if (this._errors) this._errors = undefined;
    this._isModalVisible = true;
  };

  hideModal = (): void => {
    this._isModalVisible = false;
    this.clear();
  };

  deleteRole = async (
    roleId: string
  ): Promise<undefined | ModelErrors<Errors>> => {
    const body: DeletingBody = {
      id: roleId,
    };

    const result = await post<DeletingBody, ModelErrors<Errors>>(
      "api/roles/delete",
      body
    );

    return result;
  };

  save = async (): Promise<boolean> => {
    const result = this._id ? await this.updateRole() : await this.createRole();

    if (result?.modelErrors) {
      this._errors = result.modelErrors;
      return false;
    } else {
      this.hideModal();
      this.clear();
      return true;
    }
  };

  private async createRole(): Promise<ModelErrors<Errors>> {
    const body: CreationBody = {
      name: this._name,
    };

    const result = await post<CreationBody, ModelErrors<Errors>>(
      "api/roles/create",
      body
    );

    return result;
  }

  private async updateRole(): Promise<ModelErrors<Errors>> {
    const body: UpdatingBody = {
      id: this._id!,
      name: this._name,
      concurrencyStamp: this._concurrencyStamp
    };

    const result = await post<UpdatingBody, ModelErrors<Errors>>(
      "api/roles/update",
      body
    );

    return result;
  }
}

interface CreationBody {
  name: string;
}

interface UpdatingBody extends CreationBody {
  id: string;
  concurrencyStamp: string;
}

interface DeletingBody {
  id: string;
}

interface Errors {
  id?: string;
  name?: string;
}
