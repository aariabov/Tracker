import { makeObservable, observable, computed, action } from "mobx";
import { listToTree } from "../../../helpers";
import TreeNode from "../../../interfaces/TreeNode";
import { api } from "../../../helpers/api";
import { TablePaginationConfig } from "antd";
import {
  FilterValue,
  SorterResult,
  TableCurrentDataSource,
} from "antd/lib/table/interface";
import { apiClientInstructions } from "../../../ApiClient";
import { InstructionVm } from "../../../api/InstructionsApi";

const DEFAULT_PER_PAGE = 5;

export class InstructionsStore {
  instructions: InstructionVm[] = [];
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
    const mapFunc = (instruction: InstructionVm): InstructionRow => ({
      ...instruction,
      children: [],
    });

    return listToTree(this.instructions, mapFunc);
  }

  get instructionsTreeData(): TreeNode<number>[] {
    const mapFunc = (e: InstructionVm): TreeNode<number> => ({
      key: e.id,
      value: e.id,
      title: e.name,
      parentId: e.parentId,
      children: [],
    });

    return listToTree(this.instructions, mapFunc);
  }

  async load(): Promise<void> {
    let sort: string | undefined = undefined;
    if (this.sortedInfo.order) {
      var direction =
        this.sortedInfo.order === "descend"
          ? this.sortedInfo.columnKey?.toString()
          : `-${this.sortedInfo.columnKey}`;
      sort = direction;
    }

    const query = {
      page: this.page,
      perPage: this.perPage,
      sort: sort,
    };

    const res = await api(apiClientInstructions.api.instructionsGetUserInstructionsList, query);
    this.instructions = res.data;
  }

  async getTotalInstructions(): Promise<void> {
    var res = await api(apiClientInstructions.api.instructionsTotalList);
    this.totalInstructions = res.data;
  }

  onChange = (
    pagination: TablePaginationConfig,
    filters: Record<string, FilterValue | null>,
    sorter: SorterResult<InstructionRow> | SorterResult<InstructionRow>[],
    extra: TableCurrentDataSource<InstructionRow>
  ): void => {
    this.sortedInfo = sorter as SorterResult<InstructionRow>;
    // при смене сортировки и perPage переходим на 1ую страницу
    this.page = pagination.current === this.page ? 1 : pagination.current ?? 1;
    this.perPage = pagination.pageSize ?? DEFAULT_PER_PAGE;
    this.load();
  };
}

export interface InstructionRow {
  id: number;
  name: string;
  creatorName: string;
  executorName: string;
  deadline: string;
  execDate: string | null;
  status: string;
  canCreateChild: boolean;
  canBeExecuted: boolean;
  children: InstructionRow[];
}
