import { makeObservable, observable, computed, action } from "mobx";
import { MainStore } from "./MainStore";
import { listToTree } from "../helpers";
import TreeNode from "../interfaces/TreeNode";
import { get } from "../helpers/api";

export class InstructionsStore {
  instructions: Map<number, Instruction> = new Map();
  mainStore: MainStore;

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
      deadline: new Date(instruction.deadline).toLocaleDateString("ru-RU"),
      execDate: instruction.execDate
        ? new Date(instruction.execDate).toLocaleDateString("ru-RU")
        : "",
      children: [],
    });

    return listToTree(Array.from(this.instructions.values()), mapFunc);
  }

  get instructionsTreeData(): TreeNode[] {
    const mapFunc = (e: Instruction): TreeNode => ({
      key: e.id,
      value: e.id,
      title: e.name,
      parentId: e.parentId,
      children: [],
    });

    const tree = listToTree(Array.from(this.instructions.values()), mapFunc);
    return tree;
  }

  async load(): Promise<void> {
    const data = await get<Instruction[]>("api/Instructions");
    this.instructions = new Map(data.map((el) => [el.id, el]));
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
}

export interface InstructionRow {
  id: number;
  name: string;
  parentId?: number;
  creatorName: string;
  executorName: string;
  deadline: string;
  execDate?: string;
  children: InstructionRow[];
}
