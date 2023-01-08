/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface ClientSocketRm {
  connectionId: string;
  methodName: string;
}

export interface EmployeeReportRm {
  executorId: string;
  /** @format date-time */
  startDate: string;
  /** @format date-time */
  endDate: string;
}

export interface EmployeeReportRowVm {
  id: ExecStatus;
  status: string;
  /** @format int32 */
  count: number;
}

export interface EmployeesReportRm {
  /** @format date-time */
  startDate: string;
  /** @format date-time */
  endDate: string;
}

export interface EmployeesReportRowVm {
  id: string;
  executor: string;
  /** @format int32 */
  inWorkCount: number;
  /** @format int32 */
  inWorkOverdueCount: number;
  /** @format int32 */
  completedCount: number;
  /** @format int32 */
  completedOverdueCount: number;
}

export interface ExecDateRm {
  /** @format int32 */
  instructionId: number;
  /** @format date-time */
  execDate: string;
}

/** @format int32 */
export type ExecStatus = 1 | 2 | 3 | 4;

export interface GenerationRm {
  /** @format int32 */
  total: number;
}

export interface IdentityError {
  code: string | null;
  description: string | null;
}

export interface InstructionRm {
  name: string;
  /** @format int32 */
  parentId: number | null;
  executorId: string;
  /** @format date-time */
  deadline: string;
}

export interface InstructionTreeItemVm {
  /** @format int32 */
  id: number;
  name: string;
  /** @format int32 */
  parentId: number | null;
  creatorName: string;
  executorName: string;
  /** @format date-time */
  deadline: string;
  /** @format date-time */
  execDate: string | null;
  status: string;
}

export interface InstructionVm {
  /** @format int32 */
  id: number;
  name: string;
  /** @format int32 */
  parentId: number | null;
  creatorName: string;
  executorName: string;
  /** @format date-time */
  deadline: string;
  /** @format date-time */
  execDate: string | null;
  status: string;
  canCreateChild: boolean;
  canBeExecuted: boolean;
}

export interface LoginVM {
  email: string;
  password: string;
}

export interface ModelErrorsVm {
  commonErrors: string[];
  modelErrors: Record<string, string>;
}

export interface OrgStructElementVm {
  id: string;
  name: string;
  email: string;
  parentId: string | null;
  roles: string[];
}

export interface ProblemDetails {
  type: string | null;
  title: string | null;
  /** @format int32 */
  status: number | null;
  detail: string | null;
  instance: string | null;
  [key: string]: any;
}

export interface ProgressRm {
  socketInfo: ClientSocketRm;
  /** @format int32 */
  taskId: number;
}

export interface RoleCreationRm {
  name: string;
}

export interface RoleDeletingRm {
  id: string;
}

export interface RoleUpdatingRm {
  name: string;
  id: string;
  concurrencyStamp: string;
}

export interface RoleVm {
  id: string;
  name: string;
  concurrencyStamp: string;
}

export interface TestJobParams {
  /** @format int32 */
  value: number;
}

export interface TestJobParamsProgressRm {
  socketInfo: ClientSocketRm;
  /** @format int32 */
  taskId: number;
  pars: TestJobParams;
}

export interface TokensVm {
  token: string;
  refreshToken: string;
}

export interface UserDeletingRm {
  id: string;
}

export interface UserRegistrationRm {
  name: string;
  email: string;
  bossId: string | null;
  roles: string[];
  password: string;
}

export interface UserUpdatingRm {
  name: string;
  email: string;
  bossId: string | null;
  roles: string[];
  id: string;
}

export type QueryParamsType = Record<string | number, any>;
export type ResponseFormat = keyof Omit<Body, "body" | "bodyUsed">;

export interface FullRequestParams extends Omit<RequestInit, "body"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseFormat;
  /** request body */
  body?: unknown;
  /** base url */
  baseUrl?: string;
  /** request cancellation token */
  cancelToken?: CancelToken;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> {
  baseUrl?: string;
  baseApiParams?: Omit<RequestParams, "baseUrl" | "cancelToken" | "signal">;
  securityWorker?: (securityData: SecurityDataType | null) => Promise<RequestParams | void> | RequestParams | void;
  customFetch?: typeof fetch;
}

export interface HttpResponse<D extends unknown, E extends unknown = unknown> extends Response {
  data: D;
  error: E;
}

type CancelToken = Symbol | string | number;

export enum ContentType {
  Json = "application/json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public baseUrl: string = "";
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private abortControllers = new Map<CancelToken, AbortController>();
  private customFetch = (...fetchParams: Parameters<typeof fetch>) => fetch(...fetchParams);

  private baseApiParams: RequestParams = {
    credentials: "same-origin",
    headers: {},
    redirect: "follow",
    referrerPolicy: "no-referrer",
  };

  constructor(apiConfig: ApiConfig<SecurityDataType> = {}) {
    Object.assign(this, apiConfig);
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected encodeQueryParam(key: string, value: any) {
    const encodedKey = encodeURIComponent(key);
    return `${encodedKey}=${encodeURIComponent(typeof value === "number" ? value : `${value}`)}`;
  }

  protected addQueryParam(query: QueryParamsType, key: string) {
    return this.encodeQueryParam(key, query[key]);
  }

  protected addArrayQueryParam(query: QueryParamsType, key: string) {
    const value = query[key];
    return value.map((v: any) => this.encodeQueryParam(key, v)).join("&");
  }

  protected toQueryString(rawQuery?: QueryParamsType): string {
    const query = rawQuery || {};
    const keys = Object.keys(query).filter((key) => "undefined" !== typeof query[key]);
    return keys
      .map((key) => (Array.isArray(query[key]) ? this.addArrayQueryParam(query, key) : this.addQueryParam(query, key)))
      .join("&");
  }

  protected addQueryParams(rawQuery?: QueryParamsType): string {
    const queryString = this.toQueryString(rawQuery);
    return queryString ? `?${queryString}` : "";
  }

  private contentFormatters: Record<ContentType, (input: any) => any> = {
    [ContentType.Json]: (input: any) =>
      input !== null && (typeof input === "object" || typeof input === "string") ? JSON.stringify(input) : input,
    [ContentType.Text]: (input: any) => (input !== null && typeof input !== "string" ? JSON.stringify(input) : input),
    [ContentType.FormData]: (input: any) =>
      Object.keys(input || {}).reduce((formData, key) => {
        const property = input[key];
        formData.append(
          key,
          property instanceof Blob
            ? property
            : typeof property === "object" && property !== null
            ? JSON.stringify(property)
            : `${property}`,
        );
        return formData;
      }, new FormData()),
    [ContentType.UrlEncoded]: (input: any) => this.toQueryString(input),
  };

  protected mergeRequestParams(params1: RequestParams, params2?: RequestParams): RequestParams {
    return {
      ...this.baseApiParams,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...(this.baseApiParams.headers || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected createAbortSignal = (cancelToken: CancelToken): AbortSignal | undefined => {
    if (this.abortControllers.has(cancelToken)) {
      const abortController = this.abortControllers.get(cancelToken);
      if (abortController) {
        return abortController.signal;
      }
      return void 0;
    }

    const abortController = new AbortController();
    this.abortControllers.set(cancelToken, abortController);
    return abortController.signal;
  };

  public abortRequest = (cancelToken: CancelToken) => {
    const abortController = this.abortControllers.get(cancelToken);

    if (abortController) {
      abortController.abort();
      this.abortControllers.delete(cancelToken);
    }
  };

  public request = async <T = any, E = any>({
    body,
    secure,
    path,
    type,
    query,
    format,
    baseUrl,
    cancelToken,
    ...params
  }: FullRequestParams): Promise<HttpResponse<T, E>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.baseApiParams.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const queryString = query && this.toQueryString(query);
    const payloadFormatter = this.contentFormatters[type || ContentType.Json];
    const responseFormat = format || requestParams.format;

    return this.customFetch(`${baseUrl || this.baseUrl || ""}${path}${queryString ? `?${queryString}` : ""}`, {
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type && type !== ContentType.FormData ? { "Content-Type": type } : {}),
      },
      signal: cancelToken ? this.createAbortSignal(cancelToken) : requestParams.signal,
      body: typeof body === "undefined" || body === null ? null : payloadFormatter(body),
    }).then(async (response) => {
      const r = response as HttpResponse<T, E>;
      r.data = null as unknown as T;
      r.error = null as unknown as E;

      const data = !responseFormat
        ? r
        : await response[responseFormat]()
            .then((data) => {
              if (r.ok) {
                r.data = data;
              } else {
                r.error = data;
              }
              return r;
            })
            .catch((e) => {
              r.error = e;
              return r;
            });

      if (cancelToken) {
        this.abortControllers.delete(cancelToken);
      }

      if (!response.ok) throw data;
      return data;
    });
  };
}

/**
 * @title Tracker.Web
 * @version 1.0
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
  api = {
    /**
     * No description
     *
     * @tags Analytics
     * @name AnalyticsEmployeeReport
     * @request POST:/api/Analytics/employee-report
     */
    analyticsEmployeeReport: (data: EmployeeReportRm, params: RequestParams = {}) =>
      this.request<EmployeeReportRowVm[], any>({
        path: `/api/Analytics/employee-report`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Analytics
     * @name AnalyticsEmployeesReport
     * @request POST:/api/Analytics/employees-report
     */
    analyticsEmployeesReport: (data: EmployeesReportRm, params: RequestParams = {}) =>
      this.request<EmployeesReportRowVm[], any>({
        path: `/api/Analytics/employees-report`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsList
     * @request GET:/api/Instructions
     */
    instructionsList: (
      query?: {
        /** @format int32 */
        page?: number;
        /** @format int32 */
        perPage?: number;
        /** @default "name" */
        sort?: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<InstructionVm[], any>({
        path: `/api/Instructions`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsTotalList
     * @request GET:/api/Instructions/total
     */
    instructionsTotalList: (params: RequestParams = {}) =>
      this.request<number, any>({
        path: `/api/Instructions/total`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsDetail
     * @request GET:/api/Instructions/{id}
     */
    instructionsDetail: (id: number, params: RequestParams = {}) =>
      this.request<InstructionTreeItemVm[], any>({
        path: `/api/Instructions/${id}`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsCreate
     * @request POST:/api/Instructions/create
     */
    instructionsCreate: (data: InstructionRm, params: RequestParams = {}) =>
      this.request<ModelErrorsVm, any>({
        path: `/api/Instructions/create`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsSetExecDate
     * @request POST:/api/Instructions/set-exec-date
     */
    instructionsSetExecDate: (data: ExecDateRm, params: RequestParams = {}) =>
      this.request<ModelErrorsVm, any>({
        path: `/api/Instructions/set-exec-date`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsRecalculateAllTreePaths
     * @request POST:/api/Instructions/recalculate-all-tree-paths
     */
    instructionsRecalculateAllTreePaths: (data: ProgressRm, params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/Instructions/recalculate-all-tree-paths`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsRecalculateAllClosureTable
     * @request POST:/api/Instructions/recalculate-all-closure-table
     */
    instructionsRecalculateAllClosureTable: (params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/Instructions/recalculate-all-closure-table`,
        method: "POST",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsGenerateInstructions
     * @request POST:/api/Instructions/generate-instructions
     */
    instructionsGenerateInstructions: (data: GenerationRm, params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/Instructions/generate-instructions`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Roles
     * @name RolesList
     * @request GET:/api/Roles
     */
    rolesList: (params: RequestParams = {}) =>
      this.request<RoleVm[], any>({
        path: `/api/Roles`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Roles
     * @name RolesCreate
     * @request POST:/api/Roles/create
     */
    rolesCreate: (data: RoleCreationRm, params: RequestParams = {}) =>
      this.request<ModelErrorsVm, IdentityError[]>({
        path: `/api/Roles/create`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Roles
     * @name RolesUpdate
     * @request POST:/api/Roles/update
     */
    rolesUpdate: (data: RoleUpdatingRm, params: RequestParams = {}) =>
      this.request<ModelErrorsVm, any>({
        path: `/api/Roles/update`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Roles
     * @name RolesDelete
     * @request POST:/api/Roles/delete
     */
    rolesDelete: (data: RoleDeletingRm, params: RequestParams = {}) =>
      this.request<ModelErrorsVm, any>({
        path: `/api/Roles/delete`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Test
     * @name TestRunProgressableJob
     * @request POST:/api/Test/run-progressable-job
     */
    testRunProgressableJob: (data: ProgressRm, params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/Test/run-progressable-job`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Test
     * @name TestRunProgressableJobWithParams
     * @request POST:/api/Test/run-progressable-job-with-params
     */
    testRunProgressableJobWithParams: (data: TestJobParamsProgressRm, params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/Test/run-progressable-job-with-params`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Test
     * @name TestRunUnprogressableJob
     * @request POST:/api/Test/run-unprogressable-job
     */
    testRunUnprogressableJob: (params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/Test/run-unprogressable-job`,
        method: "POST",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Test
     * @name TestRunUnprogressableJobWithParams
     * @request POST:/api/Test/run-unprogressable-job-with-params
     */
    testRunUnprogressableJobWithParams: (data: TestJobParams, params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/Test/run-unprogressable-job-with-params`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Users
     * @name UsersList
     * @request GET:/api/Users
     */
    usersList: (params: RequestParams = {}) =>
      this.request<OrgStructElementVm[], any>({
        path: `/api/Users`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Users
     * @name UsersRegister
     * @request POST:/api/Users/register
     */
    usersRegister: (data: UserRegistrationRm, params: RequestParams = {}) =>
      this.request<ModelErrorsVm, any>({
        path: `/api/Users/register`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Users
     * @name UsersUpdate
     * @request POST:/api/Users/update
     */
    usersUpdate: (data: UserUpdatingRm, params: RequestParams = {}) =>
      this.request<ModelErrorsVm, any>({
        path: `/api/Users/update`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Users
     * @name UsersDelete
     * @request POST:/api/Users/delete
     */
    usersDelete: (data: UserDeletingRm, params: RequestParams = {}) =>
      this.request<ModelErrorsVm, any>({
        path: `/api/Users/delete`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Users
     * @name UsersLogin
     * @request POST:/api/Users/login
     */
    usersLogin: (data: LoginVM, params: RequestParams = {}) =>
      this.request<TokensVm, ProblemDetails>({
        path: `/api/Users/login`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Users
     * @name UsersRefreshToken
     * @request POST:/api/Users/refresh-token
     */
    usersRefreshToken: (data: string, params: RequestParams = {}) =>
      this.request<TokensVm, ProblemDetails>({
        path: `/api/Users/refresh-token`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Users
     * @name UsersRevoke
     * @request POST:/api/Users/revoke
     */
    usersRevoke: (params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/Users/revoke`,
        method: "POST",
        ...params,
      }),
  };
}
