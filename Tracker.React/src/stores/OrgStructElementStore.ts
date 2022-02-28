import { action, makeObservable, observable } from "mobx";
import { MainStore } from "./MainStore";

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

  clear = () => {
    this._id = 0;
    this._name = "";
    this._parentId = undefined;
  };

  setName = (e: React.ChangeEvent<HTMLInputElement>) => {
    this._name = e.target.value;
  };

  setParentId = (value: number) => {
    this._parentId = value;
  };

  save = async (): Promise<void> => {
    const body = JSON.stringify({
      name: this._name,
      parentId: this._parentId,
    });

    const rawResponse = await fetch("api/OrgStruct", {
      method: "POST",
      credentials: "same-origin",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: body,
    });

    const createdElement = await rawResponse.json();
    if (createdElement.id > 0) {
      this.mainStore.orgStructStore.load();
      this.clear();
    }
  };
}
