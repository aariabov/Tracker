import { action, makeObservable, observable } from "mobx";
import moment from "moment";
import { MainStore } from "./MainStore";

export class InstructionStore {
  private _id: number = 0;
  private _name: string = "";
  private _parentId: number | undefined = undefined;
  private _creatorId: number | undefined = undefined;
  private _executorId: number | undefined = undefined;
  private _deadline: moment.Moment | null = null;

  public get name() {
    return this._name;
  }
  public get parentId() {
    return this._parentId;
  }
  public get creatorId() {
    return this._creatorId;
  }
  public get executorId() {
    return this._executorId;
  }
  public get deadline() {
    return this._deadline;
  }

  mainStore: MainStore;

  constructor(mainStore: MainStore) {
    makeObservable<
      InstructionStore,
      "_id" | "_name" | "_parentId" | "_creatorId" | "_executorId" | "_deadline"
    >(this, {
      _id: observable,
      _name: observable,
      _parentId: observable,
      _creatorId: observable,
      _executorId: observable,
      _deadline: observable,
      setName: action,
      setParentId: action,
      setCreatorId: action,
      setExecutorId: action,
      setDeadline: action,
    });

    this.mainStore = mainStore;
  }

  clear = () => {
    this._id = 0;
    this._name = "";
    this._parentId = undefined;
    this._creatorId = undefined;
    this._executorId = undefined;
    this._deadline = null;
  };

  setName = (e: React.ChangeEvent<HTMLInputElement>) => {
    this._name = e.target.value;
  };

  setParentId = (value: number) => {
    this._parentId = value;
  };

  setCreatorId = (value: number) => {
    this._creatorId = value;
  };

  setExecutorId = (value: number) => {
    this._executorId = value;
  };

  setDeadline = (momentDate: moment.Moment | null) => {
    this._deadline = momentDate;
  };

  save = async (): Promise<void> => {
    const body = JSON.stringify({
      name: this._name,
      parentId: this._parentId,
      creatorId: this._creatorId,
      executorId: this._executorId,
      deadline: this._deadline?.toDate(),
    });

    const rawResponse = await fetch("/api/Instructions", {
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
      this.mainStore.instructionsStore.load();
      this.clear();
    }
  };
}
