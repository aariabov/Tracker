import { action, makeObservable, observable } from "mobx";

export class ErrorStore {
  private _error: string | undefined = undefined;

  public get error(): string | undefined {
    return this._error;
  }

  public get hasError(): boolean {
    return this._error !== undefined;
  }

  constructor() {
    makeObservable<ErrorStore, "_error">(this, {
      _error: observable,
      setError: action,
      clearError: action,
    });
  }

  clearError = (): void => {
    this._error = undefined;
  };

  setError = (msg: string): void => {
    this._error = msg;
  };
}

export const errorStore = new ErrorStore();
