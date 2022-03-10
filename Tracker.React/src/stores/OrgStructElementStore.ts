import { action, makeObservable, observable } from "mobx";
import { post } from "../helpers/api";
import { MainStore } from "./MainStore";
import { OrgStructElement } from "./OrgStructStore";

export class OrgStructElementStore {
  private _id: number = 0;
  private _name: string = "";
  private _parentId: number | undefined = undefined;

  public get name(): string {
    return this._name;
  }
  public get parentId(): number | undefined {
    return this._parentId;
  }

  mainStore: MainStore;

  constructor(mainStore: MainStore) {
    makeObservable<OrgStructElementStore, "_name" | "_parentId">(this, {
      _name: observable,
      _parentId: observable,
      setName: action,
      setParentId: action,
    });

    this.mainStore = mainStore;
  }

  clear = (): void => {
    this._id = 0;
    this._name = "";
    this._parentId = undefined;
  };

  setName = (e: React.ChangeEvent<HTMLInputElement>): void => {
    this._name = e.target.value;
  };

  setParentId = (value: number): void => {
    this._parentId = value;
  };

  save = async (): Promise<void> => {
    const body: Body = {
      name: this._name,
      parentId: this._parentId,
    };

    const createdElement = await post<Body, OrgStructElement>(
      "api/OrgStruct",
      body
    );

    if (createdElement.id > 0) {
      this.mainStore.orgStructStore.load();
      this.clear();
    }
  };
}

interface Body {
  name: string;
  parentId: number | undefined;
}
