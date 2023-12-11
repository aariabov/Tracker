import { HubConnection } from "@microsoft/signalr";
import { HttpResponse, RequestParams, TestJobParams } from "../../api/UsersApi";
import { apiClientUsers } from "../../ApiClient";
import { api } from "../../helpers/api";
import { Status } from "./Status";

interface UtilPars {
  id: number;
  name: string;
  api: () => Promise<HttpResponse<void, any>>;
  updateUtils: () => void;
}

export abstract class Util {
  public id: number;
  public name: string;
  public status: Status;
  protected _updateUtils: () => void;

  protected abstract runJob(): Promise<void>;

  constructor(pars: UtilPars) {
    this.id = pars.id;
    this.name = pars.name;
    this._updateUtils = pars.updateUtils;
    this.status = Status.Pending;
  }

  public async run(): Promise<void> {
    this.changeStatus(Status.Processing);
    await this.runJob();
    this.changeStatus(Status.Completed);
  }

  private changeStatus(newStatus: Status): void {
    this.status = newStatus;
    this._updateUtils();
  }
}

export class UnProgressableUtil extends Util {
  private _api: () => Promise<HttpResponse<void, any>>;
  constructor(pars: UtilPars) {
    super(pars);
    this._api = pars.api;
  }

  public async runJob(): Promise<void> {
    await this._api();
  }
}

export class ProgressableUtil extends Util {
  public progress: number;
  private _api: () => Promise<HttpResponse<void, any>>;

  constructor(utilPars: UtilPars) {
    super(utilPars);

    this.progress = 0;
    this._api = utilPars.api;
  }

  public async runJob(): Promise<void> {
    this.progress = 0;
    this._updateUtils();
    await this._api();
  }

  public updateProgress(processed: number, total: number): void {
    this.progress = Math.floor((processed * 100) / total);
    this._updateUtils();
  }
}
