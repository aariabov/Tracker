import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { makeObservable, observable, action } from "mobx";
import { ProgressableUtil, UnProgressableUtil, Util } from "./Util";

interface TestParam {
    value: number;
}

interface GenerationParam {
    total: number;
}

interface ProgressResponse {
    processed: number;
    total: number;
    taskId: number;
}

const progressMethodName: string = 'progress';

export class UtilsStore {

    private _connection: HubConnection;
    private _utils: Util[] = [];

    private createUtils(): void {
        const testParam: TestParam = {
            value: 42
        };

        const generationParam: GenerationParam = {
            total: 1_000_000
        };

        this._utils = [
            new ProgressableUtil({
                id: 1,
                name: "Полный пересчет tree paths для иерархий поручений",
                url: "/api/instructions/recalculate-all-tree-paths",
                updateUtils: this.updateUtils.bind(this),
                connection: this._connection,
                progressMethodName: progressMethodName
            }),
            new UnProgressableUtil({
                id: 2,
                name: "Полный пересчет closure table для иерархий поручений",
                url: "/api/instructions/recalculate-all-closure-table",
                updateUtils: this.updateUtils.bind(this)
            }),
            new UnProgressableUtil({
                id: 3,
                name: "Генерация поручений",
                url: "/api/instructions/generate-instructions",
                updateUtils: this.updateUtils.bind(this),
                pars: generationParam,
            }),
            new ProgressableUtil({
                id: 4,
                name: "Test run progressable job",
                url: "/api/test/run-progressable-job",
                updateUtils: this.updateUtils.bind(this),
                connection: this._connection,
                progressMethodName: progressMethodName
            }),
            new ProgressableUtil({
                id: 5,
                name: "Test run progressable job with params",
                url: "/api/test/run-progressable-job-with-params",
                updateUtils: this.updateUtils.bind(this),
                pars: testParam,
                connection: this._connection,
                progressMethodName: progressMethodName
            }),
            new UnProgressableUtil({
                id: 6,
                name: "Test run unprogressable job",
                url: "/api/test/run-unprogressable-job",
                updateUtils: this.updateUtils.bind(this)
            }),
            new UnProgressableUtil({
                id: 7,
                name: "Test run unprogressable job with params",
                url: "/api/test/run-unprogressable-job-with-params",
                updateUtils: this.updateUtils.bind(this),
                pars: testParam
            }),
        ]
    };

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
        this._utils = this._utils.map(u => u);
    }

    private updateProgress = (progressResponse: ProgressResponse): void => {
        const util = this._utils.find(u => u.id === progressResponse.taskId);
        if (util) {
            const progressableUtil = util as ProgressableUtil<any>;
            progressableUtil.updateProgress(progressResponse.processed, progressResponse.total);
        }
    }

    private createConnection(): HubConnection {
        return new HubConnectionBuilder()
            .withUrl('/api/progress-hub')
            .withAutomaticReconnect()
            .build();
    }

    private startConnection(connection: HubConnection): Promise<void> {
        return connection.start()
            .then(_ => {
                console.log('Connected!');
            })
            .catch(e => console.log('Connection failed: ', e));
    }
}
