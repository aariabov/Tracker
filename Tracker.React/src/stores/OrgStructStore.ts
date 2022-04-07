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
  }

  get orgStructElementRows(): OrgStructElementRow[] {
    const mapFunc = (element: OrgStructElement): OrgStructElementRow => ({
      ...element,
      children: [],
    });

    return listToTree(this.orgStructElements, mapFunc);
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
  email: string;
  parentId?: string;
  roles: string[];
}

export interface OrgStructElementRow {
  id: string;
  name: string;
  email: string;
  parentId?: string;
  roles: string[];
  children: OrgStructElementRow[];
}
