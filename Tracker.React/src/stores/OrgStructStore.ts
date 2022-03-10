import TreeNode from "../interfaces/TreeNode";
import { makeObservable, observable, computed, action } from "mobx";
import { listToTree } from "../helpers";
import { MainStore } from "./MainStore";
import { get } from "../helpers/api";

export class OrgStructStore {
  orgStructElements: Map<number, OrgStructElement> = new Map();
  mainStore: MainStore;

  constructor(mainStore: MainStore) {
    makeObservable(this, {
      orgStructElements: observable,
      orgStructTreeData: computed,
      load: action,
    });

    this.mainStore = mainStore;
  }

  get orgStructTreeData(): TreeNode[] {
    const mapFunc = (e: OrgStructElement): TreeNode => ({
      key: e.id,
      value: e.id,
      title: e.name,
      parentId: e.parentId,
      children: [],
    });
    const tree = listToTree(
      Array.from(this.orgStructElements.values()),
      mapFunc
    );
    return tree;
  }

  getChildren = (parentId: number): OrgStructElement[] => {
    const elements = Array.from(this.orgStructElements.values());
    const children = elements.filter((e) => e.parentId === parentId);
    return children;
  };

  load = async (): Promise<void> => {
    const data = await get<OrgStructElement[]>("api/OrgStruct");
    this.orgStructElements = new Map(data.map((el) => [el.id, el]));
  };
}

export interface OrgStructElement {
  id: number;
  name: string;
  parentId?: number;
}
