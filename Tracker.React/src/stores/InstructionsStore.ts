import { makeObservable, observable, computed, action } from "mobx";
import { MainStore } from "./MainStore";
import { listToTree } from "../helpers";
import TreeNode from "../interfaces/TreeNode";
import { get } from "../helpers/api";

export class InstructionsStore {
  private readonly mainStore: MainStore;

  instructions: Instruction[] = [];

  constructor(mainStore: MainStore) {
    makeObservable(this, {
      instructions: observable,
      instructionsRows: computed,
      load: action,
    });

    this.mainStore = mainStore;
  }

  get instructionsRows(): InstructionRow[] {
    const mapFunc = (instruction: Instruction): InstructionRow => ({
      ...instruction,
      children: [],
    });

    return listToTree(this.instructions, mapFunc);
  }

  get instructionsTreeData(): TreeNode<number>[] {
    const mapFunc = (e: Instruction): TreeNode<number> => ({
      key: e.id,
      value: e.id,
      title: e.name,
      parentId: e.parentId,
      children: [],
    });

    return listToTree(this.instructions, mapFunc);
  }

  async load(): Promise<void> {
    this.instructions = await get<Instruction[]>("api/Instructions");
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
  canCreateChild: boolean;
  canBeExecuted: boolean;
}

export interface InstructionRow {
  id: number;
  name: string;
  creatorName: string;
  executorName: string;
  deadline: Date;
  execDate?: Date;
  status: string;
  canCreateChild: boolean;
  canBeExecuted: boolean;
  children: InstructionRow[];
}
