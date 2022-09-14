import { makeObservable, observable, action } from "mobx";
import { get, post } from "../../helpers/api";
import { Status } from "./Status";

export class UtilsStore {
    private _utils: Util[] = [
        {
            id: 1,
            name: "Полный пересчет tree paths для иерархий поручений",
            status: Status.Pending,
            run: async () => {
                await post("api/instructions/recalculate-all-tree-paths", undefined);
            }
        },
        {
            id: 2,
            name: "Полный пересчет closure table для иерархий поручений",
            status: Status.Pending,
            run: async () => {
                await post("api/instructions/recalculate-all-closure-table", undefined);
            }
        },
        {
            id: 3,
            name: "Генерация поручений",
            status: Status.Pending,
            run: async () => {
                await post("api/instructions/generate-instructions", undefined);
            }
        }
    ];

    public get utils(): Util[] {
        return this._utils;
    }

    constructor() {
        makeObservable<UtilsStore, "_utils">(this, {
            _utils: observable,
            run: action,
        });
    }

    run = async (util: Util): Promise<void> => {
        this.changeStatus(util, Status.Processing);
        await util.run();
        this.changeStatus(util, Status.Completed);
    };

    private changeStatus(util: Util, newStatus: Status) {
        util.status = newStatus;
        this._utils = this._utils.map(u => u.id === util.id ? util : u);
    }
}

export interface Util {
    id: number;
    name: string;
    status: Status;
    run: () => Promise<void>;
}
