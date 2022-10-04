import { makeObservable, observable, computed, action } from "mobx";
import { listToTree } from "../../../helpers";
import TreeNode from "../../../interfaces/TreeNode";
import { get } from "../../../helpers/api";
import { TablePaginationConfig } from "antd";
import { FilterValue, SorterResult, TableCurrentDataSource } from "antd/lib/table/interface";

const DEFAULT_PER_PAGE = 5;

export class InstructionsStore {
  instructions: Instruction[] = [];
  totalInstructions: number = 0;
  page: number = 1;
  perPage: number = DEFAULT_PER_PAGE;
  sortedInfo: SorterResult<InstructionRow> = {};

  constructor() {
    makeObservable(this, {
      instructions: observable,
      totalInstructions: observable,
      instructionsRows: computed,
      load: action,
    });
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
    var url = `api/Instructions?page=${this.page}&perPage=${this.perPage}`;
    if (this.sortedInfo.order) {
      var direction = this.sortedInfo.order === 'descend'
        ? this.sortedInfo.columnKey
        : `-${this.sortedInfo.columnKey}`;
      url += `&sort=${direction}`
    }
    this.instructions = await get<Instruction[]>(url);
  }

  async getTotalInstructions(): Promise<void> {
    this.totalInstructions = await get<number>("api/Instructions/total");
  }

  onChange = (pagination: TablePaginationConfig, filters: Record<string, FilterValue | null>
    , sorter: SorterResult<InstructionRow> | SorterResult<InstructionRow>[]
    , extra: TableCurrentDataSource<InstructionRow>): void => {
    this.sortedInfo = sorter as SorterResult<InstructionRow>;
    // при смене сортировки и perPage переходим на 1ую страницу
    this.page = pagination.current === this.page ? 1 : (pagination.current ?? 1);
    this.perPage = pagination.pageSize ?? DEFAULT_PER_PAGE;
    this.load();
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
