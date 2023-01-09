import { makeObservable, observable, action } from "mobx";
import { RoleVm } from "../../api/Api";
import { apiClient } from "../../ApiClient";
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
    var res = await api(apiClient.api.rolesList);
    this._roles = res.data;
  };
}
