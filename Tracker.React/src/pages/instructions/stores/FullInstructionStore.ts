import { makeObservable, observable, computed, action } from "mobx";
import { InstructionTreeItemVm } from "../../../api/InstructionsApi";
import { apiClientInstructions } from "../../../ApiClient";
import { listToTree } from "../../../helpers";
import { api } from "../../../helpers/api";

export class FullInstructionStore {
  private _instructions: InstructionTreeItemVm[] = [];
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
    const mapFunc = (instruction: InstructionTreeItemVm): InstructionRow => ({
      ...instruction,
      children: [],
    });

    return listToTree(this._instructions, mapFunc);
  }

  show = async (instructionId: number): Promise<void> => {
    this._isDrawerVisible = true;
    const res = await api(apiClientInstructions.api.instructionsDetail, instructionId);
    this._instructions = res.data;
  };

  close = (): void => {
    this._isDrawerVisible = false;
    this._instructions = [];
  };
}

export interface InstructionRow {
  id: number;
  name: string;
  creatorName: string;
  executorName: string;
  deadline: string;
  execDate: string | null;
  children: InstructionRow[];
  status: string;
}
