import { InstructionsStore } from "./InstructionsStore";
import { InstructionStore } from "./InstructionStore";
import { OrgStructStore } from "./OrgStructStore";
import { OrgStructElementStore } from "./OrgStructElementStore";

export class MainStore {
  instructionsStore: InstructionsStore;
  instructionStore: InstructionStore;
  orgStructStore: OrgStructStore;
  orgStructElementStore: OrgStructElementStore;

  constructor() {
    this.instructionsStore = new InstructionsStore(this);
    this.instructionStore = new InstructionStore(this);
    this.orgStructStore = new OrgStructStore(this);
    this.orgStructElementStore = new OrgStructElementStore(this);
  }
}

export const mainStore = new MainStore();
