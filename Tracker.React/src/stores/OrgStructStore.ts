import { makeObservable, observable, computed, action } from "mobx";
import { listToTree } from "../helpers";
import { get } from "../helpers/api";
import TreeNode from "../interfaces/TreeNode";

export class OrgStructStore {
  orgStructElements: OrgStructElement[] = [];

  constructor() {
    makeObservable(this, {
      orgStructElements: observable,
      orgStructTreeData: computed,
      load: action,
    });

    this.load();
  }

  get orgStructTreeData(): TreeNode<string>[] {
    const mapFunc = (e: OrgStructElement): TreeNode<string> => ({
      key: e.id,
      value: e.id,
      title: e.name,
      parentId: e.parentId,
      children: [],
    });

    return listToTree(this.orgStructElements, mapFunc);
  }

  getChildren = (parentId: string): OrgStructElement[] => {
    return this.orgStructElements.filter((e) => e.parentId === parentId);
  };

  load = async (): Promise<void> => {
    this.orgStructElements = await get<OrgStructElement[]>("api/OrgStruct");
  };
}

export interface OrgStructElement {
  id: string;
  name: string;
  parentId?: string;
}
