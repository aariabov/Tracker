import { makeObservable, observable, computed, action } from "mobx";
import { Moment } from "moment";
import { RangeValue } from "rc-picker/lib/interface";
import { post } from "../../helpers/api";

export class EmployeeReportStore {
  private _rows: ReportRow[] = [];

  private _executorId: string | undefined = undefined;
  private _period: RangeValue<Moment> = null;

  private _executorIdError: string | undefined = undefined;
  private _periodError: string | undefined = undefined;

  public get rows(): ReportRow[] {
    return this._rows;
  }

  public get executorIdError(): string | undefined {
    return this._executorIdError;
  }
  public get periodError(): string | undefined {
    return this._periodError;
  }

  constructor() {
    makeObservable<
      EmployeeReportStore,
      "_executorId" | "_period" | "_executorIdError" | "_periodError" | "_rows"
    >(this, {
      _rows: observable,
      _executorId: observable,
      _period: observable,
      _executorIdError: observable,
      _periodError: observable,
    });
  }

  setExecutorId = (value: string): void => {
    this._executorId = value;
    this._executorIdError = undefined;
    this._rows = [];
  };

  setPeriod = (values: RangeValue<Moment>): void => {
    this._period = values;
    this._periodError = undefined;
    this._rows = [];
  };

  load = async (): Promise<void> => {
    if (
      this._executorId &&
      this._period &&
      this._period[0] &&
      this._period[1]
    ) {
      const body: Body = {
        executorId: this._executorId,
        startDate: this._period[0].toDate(),
        endDate: this._period[1].toDate(),
      };

      this._rows = await post<Body, ReportRow[]>(
        "api/analytics/employee-report",
        body
      );
    } else {
      if (!this._executorId) this._executorIdError = "Выберите сотрудника";
      if (!this._period) this._periodError = "Выберите период";
    }
  };
}

interface Body {
  executorId: string;
  startDate: Date;
  endDate: Date;
}

export interface ReportRow {
  id: string;
  status: string;
  count: number;
}
