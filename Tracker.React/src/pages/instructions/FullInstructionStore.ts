import { makeObservable, observable, computed, action } from "mobx";
import { listToTree } from "../../helpers";
import { get, post } from "../../helpers/api";

export class FullInstructionStore {
  private _instructions: Instruction[] = [];
  private _isDrawerVisible: boolean = false;

  constructor() {
    makeObservable<FullInstructionStore, "_instructions" | "_isDrawerVisible">(
      this,
      {
        _instructions: observable,
        _isDrawerVisible: observable,
        instructionsRows: computed,
        show: action,
        close: action,
      }
    );
  }

  public get isDrawerVisible(): boolean {
    return this._isDrawerVisible;
  }

  get instructionsRows(): InstructionRow[] {
    const mapFunc = (instruction: Instruction): InstructionRow => ({
      ...instruction,
      children: [],
    });

    return listToTree(this._instructions, mapFunc);
  }

  show = async (instructionId: number): Promise<void> => {
    this._isDrawerVisible = true;
    this._instructions = await get<Instruction[]>(
      `api/Instructions/${instructionId}`
    );
  };

  close = (): void => {
    this._isDrawerVisible = false;
    this._instructions = [];
  };

  async setExecutionDate(instructionId: number, execDate: Date): Promise<void> {
    await post<Date>(`api/Instructions/${instructionId}`, execDate);
  }
}

export interface Instruction {
  id: number;
  name: string;
  parentId?: number;
  creatorName: string;
  executorName: string;
  deadline: Date;
  execDate?: Date;
  status: string;
}

export interface InstructionRow {
  id: number;
  name: string;
  creatorName: string;
  executorName: string;
  deadline: Date;
  execDate?: Date;
  children: InstructionRow[];
  status: string;
}
