import { makeObservable, observable, computed, action } from "mobx";
import { Moment } from "moment";
import { RangeValue } from "rc-picker/lib/interface";
import { EmployeeReportRm, EmployeeReportRowVm } from "../../api/AnalyticsApi";
import { apiClientAnalytics } from "../../ApiClient";
import { api } from "../../helpers/api";

export class EmployeeReportStore {
  private _rows: EmployeeReportRowVm[] = [];

  private _executorId: string | undefined = undefined;
  private _period: RangeValue<Moment> = null;

  private _executorIdError: string | undefined = undefined;
  private _periodError: string | undefined = undefined;

  public get rows(): EmployeeReportRowVm[] {
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
      const body: EmployeeReportRm = {
        executorId: this._executorId,
        startDate: this._period[0].toDate().toISOString(),
        endDate: this._period[1].toDate().toISOString(),
      };

      const res = await api(apiClientAnalytics.api.analyticsEmployeeReport, body);
      this._rows = res.data;
    } else {
      if (!this._executorId) this._executorIdError = "Выберите сотрудника";
      if (!this._period) this._periodError = "Выберите период";
    }
  };
}
