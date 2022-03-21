import { OrgStructStore } from "../../../stores/OrgStructStore";
import { FullInstructionStore } from "./FullInstructionStore";
import { InstructionsStore } from "./InstructionsStore";
import { InstructionStore } from "./InstructionStore";

export class PageStore {
  instructionsStore: InstructionsStore;
  instructionStore: InstructionStore;
  orgStructStore: OrgStructStore;
  fullInstructionStore: FullInstructionStore;

  constructor() {
    this.instructionsStore = new InstructionsStore(this);
    this.instructionStore = new InstructionStore(this);
    this.fullInstructionStore = new FullInstructionStore();
    this.orgStructStore = new OrgStructStore();
  }
}

export const pageStore = new PageStore();
