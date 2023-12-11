import { action, makeObservable, observable } from "mobx";
import { ExecDateRm, InstructionRm } from "../../../api/InstructionsApi";
import { apiClientInstructions } from "../../../ApiClient";
import { api, ModelErrors } from "../../../helpers/api";

export class InstructionStore {
  private _id: number = 0;
  private _name: string = "";
  private _parentId: number | null = null;
  private _executorId: string | undefined = undefined;
  private _deadline: Date | undefined = undefined;

  private _isModalVisible: boolean = false;
  private _errors: Errors | undefined = undefined;

  public get name(): string {
    return this._name;
  }
  public get parentId(): number | null {
    return this._parentId;
  }
  public get executorId(): string | undefined {
    return this._executorId;
  }
  public get deadline(): Date | undefined {
    return this._deadline;
  }
  public get errors(): Errors | undefined {
    return this._errors;
  }
  public get isModalVisible(): boolean {
    return this._isModalVisible;
  }

  /**
   * @todo https://tfs.parma.ru/tfs/PARMA/EDU/_git/Riabov.AA/pullrequest/141456?_a=files&path=%2FTracker.React%2Fsrc%2Fstores%2FInstructionStore.ts&discussionId=945688
   */
  constructor() {
    makeObservable<
      InstructionStore,
      | "_id"
      | "_name"
      | "_parentId"
      | "_executorId"
      | "_deadline"
      | "_errors"
      | "_isModalVisible"
    >(this, {
      _id: observable,
      _name: observable,
      _parentId: observable,
      _executorId: observable,
      _deadline: observable,
      _errors: observable,
      _isModalVisible: observable,
      setName: action,
      setParentId: action,
      setExecutorId: action,
      setDeadline: action,
    });
  }

  clear = (): void => {
    this._id = 0;
    this._name = "";
    this._parentId = null;
    this._executorId = undefined;
    this._deadline = undefined;
    this._errors = undefined;
  };

  setName = (value: string): void => {
    this._name = value;
    if (this._errors?.name) this._errors.name = undefined;
  };

  setParentId = (value: number): void => {
    this._parentId = value;
    if (this._errors?.parentId) this._errors.parentId = undefined;
  };

  setExecutorId = (value: string): void => {
    this._executorId = value;
    if (this._errors?.executorId) this._errors.executorId = undefined;
  };

  setDeadline = (value: Date | undefined): void => {
    this._deadline = value;
    if (this._errors?.deadline) this._errors.deadline = undefined;
  };

  showModal = (): void => {
    this._isModalVisible = true;
  };

  hideModal = (): void => {
    this._isModalVisible = false;
    this.clear();
  };

  async setExecutionDate(instructionId: number, execDate: Date): Promise<void> {
    const body: ExecDateRm = {
      instructionId: instructionId,
      execDate: execDate.toISOString(),
    };

    await api(apiClientInstructions.api.instructionsSetExecDate, body);
  }

  save = async (): Promise<boolean> => {
    const body: InstructionRm = {
      name: this._name,
      parentId: this._parentId,
      executorId: this._executorId ?? "",
      deadline: this._deadline?.toISOString() ?? "",
    };

    var result = await api(apiClientInstructions.api.instructionsCreate, body);
    if (result.data.modelErrors) {
      this._errors = result.data.modelErrors;
      return false;
    } else {
      this.hideModal();
      this.clear();
      return true;
    }
  };
}

interface Errors {
  name?: string;
  parentId?: string;
  executorId?: string;
  deadline?: string;
}
