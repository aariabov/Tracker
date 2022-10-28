import { HubConnection } from '@microsoft/signalr';
import { post } from "../../helpers/api";
import { Status } from "./Status";

interface UtilPars {
    id: number,
    name: string,
    url: string,
    updateUtils: () => void
}

interface UnProgressableUtilPars<T> extends UtilPars {
    pars?: T
}

interface ProgressableUtilPars<T> extends UtilPars {
    connection: HubConnection,
    progressMethodName: string;
    pars?: T
}

export abstract class Util {
    public id: number;
    public name: string;
    public status: Status;
    protected _url: string;
    protected _updateUtils: () => void;

    protected abstract runJob(): Promise<void>;

    constructor(pars: UtilPars) {
        this.id = pars.id;
        this.name = pars.name;
        this._url = pars.url;
        this._updateUtils = pars.updateUtils;
        this.status = Status.Pending;
    }

    public async run(): Promise<void> {
        this.changeStatus(Status.Processing);
        await this.runJob();
        this.changeStatus(Status.Completed);
    }

    private changeStatus(newStatus: Status) {
        this.status = newStatus;
        this._updateUtils();
    }
}

export class UnProgressableUtil<T> extends Util {
    private _pars?: T;
    constructor(utilPars: UnProgressableUtilPars<T>) {
        super(utilPars);
        this._pars = utilPars.pars;
    }

    public async runJob(): Promise<void> {
        await post(this._url, this._pars);
    }
}

interface SocketBody {
    connectionId: string;
    methodName: string;
}

interface RequestBody<T> {
    socketInfo: SocketBody;
    taskId: number;
    pars?: T;
}

export class ProgressableUtil<T> extends Util {
    public progress: number;
    private _connection: HubConnection;
    private _progressMethodName: string;
    private _pars?: T;

    constructor(utilPars: ProgressableUtilPars<T>) {
        super(utilPars);

        this.progress = 0;
        this._pars = utilPars.pars;
        this._connection = utilPars.connection;
        this._progressMethodName = utilPars.progressMethodName;
    }

    public async runJob(): Promise<void> {
        this.progress = 0;
        this._updateUtils();

        if (!this._connection.connectionId)
            throw new Error('ConnectionId is null')

        var body: RequestBody<T> = {
            socketInfo: {
                connectionId: this._connection.connectionId,
                methodName: this._progressMethodName
            },
            taskId: this.id,
            pars: this._pars
        }

        await post(this._url, body);
    }

    public updateProgress(processed: number, total: number) {
        this.progress = Math.floor((processed * 100) / total);
        this._updateUtils();
    }
}