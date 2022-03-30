import { makeObservable, observable, computed, action } from "mobx";
import { Moment } from "moment";
import { RangeValue } from "rc-picker/lib/interface";
import { post } from "../../helpers/api";

export class EmployeesReportStore {
  private _rows: ReportRow[] = [];
  private _period: RangeValue<Moment> = null;

  private _periodError: string | undefined = undefined;

  public get rows(): ReportRow[] {
    return this._rows;
  }
  public get periodError(): string | undefined {
    return this._periodError;
  }

  constructor() {
    makeObservable<EmployeesReportStore, "_period" | "_periodError" | "_rows">(
      this,
      {
        _rows: observable,
        _period: observable,
        _periodError: observable,
      }
    );
  }

  setPeriod = (values: RangeValue<Moment>): void => {
    this._period = values;
    this._periodError = undefined;
    this._rows = [];
  };

  load = async (): Promise<void> => {
    if (this._period && this._period[0] && this._period[1]) {
      const body: Body = {
        startDate: this._period[0].toDate(),
        endDate: this._period[1].toDate(),
      };

      this._rows = await post<Body, ReportRow[]>(
        "api/analytics/employees-report",
        body
      );
    } else {
      if (!this._period) this._periodError = "Выберите период";
    }
  };
}

interface Body {
  startDate: Date;
  endDate: Date;
}

export interface ReportRow {
  id: string;
  executor: string;
  inWorkCount: number;
  inWorkOverdueCount: number;
  completedCount: number;
  completedOverdueCount: number;
}
