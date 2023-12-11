import { makeObservable, observable, action } from "mobx";
import { RoleVm } from "../../api/UsersApi";
import { apiClientUsers } from "../../ApiClient";
import { api } from "../../helpers/api";

export class RolesStore {
  private _roles: RoleVm[] = [];

  public get roles(): RoleVm[] {
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
    var res = await api(apiClientUsers.api.rolesGetAllRolesList);
    this._roles = res.data;
  };
}
