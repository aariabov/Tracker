import { action, makeObservable, observable } from "mobx";
import moment from "moment";
import { post } from "../helpers/api";
import { Instruction } from "./InstructionsStore";
import { MainStore } from "./MainStore";

export class InstructionStore {
  private _id: number = 0;
  private _name: string = "";
  private _parentId: number | undefined = undefined;
  private _creatorId: number | undefined = undefined;
  private _executorId: number | undefined = undefined;
  private _deadline: moment.Moment | null = null;

  public get name(): string {
    return this._name;
  }
  public get parentId(): number | undefined {
    return this._parentId;
  }
  public get creatorId(): number | undefined {
    return this._creatorId;
  }
  public get executorId(): number | undefined {
    return this._executorId;
  }
  public get deadline(): moment.Moment | null {
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

  clear = (): void => {
    this._id = 0;
    this._name = "";
    this._parentId = undefined;
    this._creatorId = undefined;
    this._executorId = undefined;
    this._deadline = null;
  };

  setName = (e: React.ChangeEvent<HTMLInputElement>): void => {
    this._name = e.target.value;
  };

  setParentId = (value: number): void => {
    this._parentId = value;
  };

  setCreatorId = (value: number): void => {
    this._creatorId = value;
  };

  setExecutorId = (value: number): void => {
    this._executorId = value;
  };

  setDeadline = (momentDate: moment.Moment | null): void => {
    this._deadline = momentDate;
  };

  save = async (): Promise<void> => {
    const body: InstructionBody = {
      name: this._name,
      parentId: this._parentId,
      creatorId: this._creatorId,
      executorId: this._executorId,
      deadline: this._deadline?.toDate(),
    };

    const createdElement = await post<InstructionBody, Instruction>(
      "api/Instructions",
      body
    );

    if (createdElement.id > 0) {
      this.mainStore.instructionsStore.load();
      this.clear();
    }
  };
}

interface InstructionBody {
  name: string;
  parentId: number | undefined;
  creatorId: number | undefined;
  executorId: number | undefined;
  deadline: Date | undefined;
}
