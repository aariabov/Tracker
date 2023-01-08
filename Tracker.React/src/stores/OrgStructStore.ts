import { makeObservable, observable, computed, action } from "mobx";
import { OrgStructElementVm } from "../api/Api";
import { apiClient } from "../ApiClient";
import { listToTree } from "../helpers";
import { api } from "../helpers/api";
import TreeNode from "../interfaces/TreeNode";

export class OrgStructStore {
  orgStructElements: OrgStructElementVm[] = [];

  constructor() {
    makeObservable(this, {
      orgStructElements: observable,
      orgStructTreeData: computed,
      load: action,
    });
  }

  get orgStructElementRows(): OrgStructElementRow[] {
    const mapFunc = (element: OrgStructElementVm): OrgStructElementRow => ({
      ...element,
      children: [],
    });

    return listToTree(this.orgStructElements, mapFunc);
  }

  get orgStructTreeData(): TreeNode<string>[] {
    const mapFunc = (e: OrgStructElementVm): TreeNode<string> => ({
      key: e.id,
      value: e.id,
      title: e.name,
      parentId: e.parentId,
      children: [],
    });

    return listToTree(this.orgStructElements, mapFunc);
  }

  getChildren = (parentId: string): OrgStructElementVm[] => {
    return this.orgStructElements.filter((e) => e.parentId === parentId);
  };

  load = async (): Promise<void> => {
    var res = await api(apiClient.api.usersList);
    this.orgStructElements = res.data;
  };
}

export interface OrgStructElementRow {
  id: string;
  name: string;
  email: string;
  parentId: string | null;
  roles: string[];
  children: OrgStructElementRow[];
}
