import { makeObservable, observable, action } from "mobx";
import { get } from "../../helpers/api";

export class RolesStore {
  private _roles: Role[] = [];

  public get roles(): Role[] {
    return this._roles;
  }

  constructor() {
    makeObservable<RolesStore, "_roles">(this, {
      _roles: observable,
      load: action,
    });
  }

  load = async (): Promise<void> => {
    this._roles = [];
    this._roles = await get<Role[]>("api/roles");
  };
}

export interface Role {
  id: string;
  name: string;
  concurrencyStamp: string;
}
