import { makeObservable, observable, computed, action } from "mobx";
import { Moment } from "moment";
import { RangeValue } from "rc-picker/lib/interface";
import { EmployeesReportRm, EmployeesReportRowVm } from "../../api/AnalyticsApi";
import { apiClientAnalytics } from "../../ApiClient";
import { api } from "../../helpers/api";

export class EmployeesReportStore {
  private _rows: EmployeesReportRowVm[] = [];
  private _period: RangeValue<Moment> = null;

  private _periodError: string | undefined = undefined;

  public get rows(): EmployeesReportRowVm[] {
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
      const body: EmployeesReportRm = {
        startDate: this._period[0].toDate().toISOString(),
        endDate: this._period[1].toDate().toISOString(),
      };

      const res = await api(apiClientAnalytics.api.analyticsEmployeesReport, body);
      this._rows = res.data;
    } else {
      if (!this._period) this._periodError = "Выберите период";
    }
  };
}
