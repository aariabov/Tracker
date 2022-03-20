import { action, makeObservable, observable } from "mobx";
import moment from "moment";
import { post } from "../helpers/api";
import { Instruction } from "./InstructionsStore";
import { MainStore } from "./MainStore";

export class InstructionStore {
  private readonly mainStore: MainStore;

  private _id: number = 0;
  private _name: string = "";
  private _parentId: number | undefined = undefined;
  private _executorId: string | undefined = undefined;
  private _deadline: moment.Moment | null = null;
  private _isModalVisible: boolean = false;

  public get name(): string {
    return this._name;
  }
  public get parentId(): number | undefined {
    return this._parentId;
  }
  public get executorId(): string | undefined {
    return this._executorId;
  }
  public get deadline(): moment.Moment | null {
    return this._deadline;
  }
  public get isModalVisible(): boolean {
    return this._isModalVisible;
  }

  /**
   * @todo https://tfs.parma.ru/tfs/PARMA/EDU/_git/Riabov.AA/pullrequest/141456?_a=files&path=%2FTracker.React%2Fsrc%2Fstores%2FInstructionStore.ts&discussionId=945688
   */
  constructor(mainStore: MainStore) {
    makeObservable<
      InstructionStore,
      | "_id"
      | "_name"
      | "_parentId"
      | "_executorId"
      | "_deadline"
      | "_isModalVisible"
    >(this, {
      _id: observable,
      _name: observable,
      _parentId: observable,
      _executorId: observable,
      _deadline: observable,
      _isModalVisible: observable,
      setName: action,
      setParentId: action,
      setExecutorId: action,
      setDeadline: action,
    });

    this.mainStore = mainStore;
  }

  clear = (): void => {
    this._id = 0;
    this._name = "";
    this._parentId = undefined;
    this._executorId = undefined;
    this._deadline = null;
  };

  setName = (e: React.ChangeEvent<HTMLInputElement>): void => {
    this._name = e.target.value;
  };

  setParentId = (value: number): void => {
    this._parentId = value;
  };

  setExecutorId = (value: string): void => {
    this._executorId = value;
  };

  setDeadline = (momentDate: moment.Moment | null): void => {
    this._deadline = momentDate;
  };

  showModal = (): void => {
    this._isModalVisible = true;
  };

  hideModal = (): void => {
    this._isModalVisible = false;
    this.clear();
  };

  save = async (): Promise<void> => {
    if (this._executorId && this._deadline) {
      const body: InstructionBody = {
        name: this._name,
        parentId: this._parentId,
        executorId: this._executorId,
        deadline: this._deadline?.toDate(),
      };

      const createdElement = await post<InstructionBody, Instruction>(
        "api/Instructions",
        body
      );

      if (createdElement.id > 0) {
        this.mainStore.instructionsStore.load();
        this.hideModal();
        this.clear();
      }
    }
  };
}

interface InstructionBody {
  name: string;
  parentId: number | undefined;
  executorId: string;
  deadline: Date;
}
