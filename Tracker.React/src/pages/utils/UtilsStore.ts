import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { makeObservable, observable, action } from "mobx";
import { GenerationRm } from "../../api/InstructionsApi";
import { TestJobParams } from "../../api/UsersApi";
import { apiClientInstructions, apiClientUsers } from "../../ApiClient";
import { api } from "../../helpers/api";
import { ProgressableUtil, UnProgressableUtil, Util } from "./Util";

interface ProgressResponse {
  processed: number;
  total: number;
  taskId: number;
}

const progressMethodName: string = "progress";

export class UtilsStore {
  private _connection: HubConnection;
  private _utils: Util[] = [];

  private createUtils(): void {
    const testParam: TestJobParams = {
      value: 42,
    };

    const generationParam: GenerationRm = {
      total: 1_000_000,
    };

    this._utils = [
      new ProgressableUtil({
        id: 1,
        name: "Полный пересчет tree paths для иерархий поручений",
        updateUtils: this.updateUtils.bind(this),
        api: () =>
          api(apiClientInstructions.api.instructionsRecalculateAllTreePaths, {
            socketInfo: {
              connectionId: this._connection.connectionId!,
              methodName: progressMethodName,
            },
            taskId: 1,
          }),
      }),
      new UnProgressableUtil({
        id: 2,
        name: "Полный пересчет closure table для иерархий поручений",
        updateUtils: this.updateUtils.bind(this),
        api: () => api(apiClientInstructions.api.instructionsRecalculateAllClosureTable),
      }),
      new UnProgressableUtil({
        id: 3,
        name: "Генерация поручений",
        updateUtils: this.updateUtils.bind(this),
        api: () =>
          api(apiClientInstructions.api.instructionsGenerateInstructions, generationParam),
      }),
      new ProgressableUtil({
        id: 4,
        name: "Test run progressable job",
        updateUtils: this.updateUtils.bind(this),
        api: () =>
          api(apiClientUsers.api.testRunProgressableJob, {
            socketInfo: {
              connectionId: this._connection.connectionId!,
              methodName: progressMethodName,
            },
            taskId: 4,
          }),
      }),
      new ProgressableUtil({
        id: 5,
        name: "Test run progressable job with params",
        updateUtils: this.updateUtils.bind(this),
        api: () =>
          api(apiClientUsers.api.testRunProgressableJobWithParams, {
            socketInfo: {
              connectionId: this._connection.connectionId!,
              methodName: progressMethodName,
            },
            taskId: 5,
            pars: testParam,
          }),
      }),
      new UnProgressableUtil({
        id: 6,
        name: "Test run unprogressable job",
        api: () => api(apiClientUsers.api.testRunUnprogressableJob),
        updateUtils: this.updateUtils.bind(this),
      }),
      new UnProgressableUtil({
        id: 7,
        name: "Test run unprogressable job with params",
        updateUtils: this.updateUtils.bind(this),
        api: () =>
          api(apiClientUsers.api.testRunUnprogressableJobWithParams, testParam),
      }),
      new UnProgressableUtil({
        id: 8,
        name: "Пересчитать все статусы",
        updateUtils: this.updateUtils.bind(this),
        api: () => api(apiClientInstructions.api.instructionsRecalculateAllStatuses),
      }),
      new UnProgressableUtil({
        id: 9,
        name: "Пересчитать статусы для поручений в работе и дедлайне",
        updateUtils: this.updateUtils.bind(this),
        api: () => api(apiClientInstructions.api.instructionsRecalculateStatusesForInWorkAndDeadline),
      }),
    ];
  }

  public get utils(): Util[] {
    return this._utils;
  }

  constructor() {
    makeObservable<UtilsStore, "_utils">(this, {
      _utils: observable,
    });

    this._connection = this.createConnection();
    this.startConnection(this._connection);
    this._connection.on(progressMethodName, this.updateProgress);
    this.createUtils();
  }

  public updateUtils(): void {
    this._utils = this._utils.map((u) => u);
  }

  private updateProgress = (progressResponse: ProgressResponse): void => {
    const util = this._utils.find((u) => u.id === progressResponse.taskId);
    if (util) {
      const progressableUtil = util as ProgressableUtil;
      progressableUtil.updateProgress(
        progressResponse.processed,
        progressResponse.total
      );
    }
  };

  private createConnection(): HubConnection {
    return new HubConnectionBuilder()
      .withUrl("/signalr/progress-hub")
      .withAutomaticReconnect()
      .build();
  }

  private startConnection(connection: HubConnection): Promise<void> {
    return connection
      .start()
      .then((_) => {
        console.log("Connected!");
      })
      .catch((e) => console.log("Connection failed: ", e));
  }
}
