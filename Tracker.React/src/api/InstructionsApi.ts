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

export interface ExecDateRm {
  /** @format int32 */
  instructionId: number;
  /** @format date-time */
  execDate: string;
}

export interface GenerationRm {
  /** @format int32 */
  total: number;
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

export interface ModelErrorsVm {
  commonErrors: string[];
  modelErrors: Record<string, string>;
}

export interface ProgressRm {
  socketInfo: ClientSocketRm;
  /** @format int32 */
  taskId: number;
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
 * @title Tracker.Instructions
 * @version 1.0
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
  api = {
    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsGetUserInstructionsList
     * @request GET:/api/instructions/get-user-instructions
     */
    instructionsGetUserInstructionsList: (
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
        path: `/api/instructions/get-user-instructions`,
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
     * @request GET:/api/instructions/total
     */
    instructionsTotalList: (params: RequestParams = {}) =>
      this.request<number, any>({
        path: `/api/instructions/total`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsDetail
     * @request GET:/api/instructions/{id}
     */
    instructionsDetail: (id: number, params: RequestParams = {}) =>
      this.request<InstructionTreeItemVm[], any>({
        path: `/api/instructions/${id}`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsCreate
     * @request POST:/api/instructions/create
     */
    instructionsCreate: (data: InstructionRm, params: RequestParams = {}) =>
      this.request<ModelErrorsVm, any>({
        path: `/api/instructions/create`,
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
     * @request POST:/api/instructions/set-exec-date
     */
    instructionsSetExecDate: (data: ExecDateRm, params: RequestParams = {}) =>
      this.request<ModelErrorsVm, any>({
        path: `/api/instructions/set-exec-date`,
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
     * @request POST:/api/instructions/recalculate-all-tree-paths
     */
    instructionsRecalculateAllTreePaths: (data: ProgressRm, params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/instructions/recalculate-all-tree-paths`,
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
     * @request POST:/api/instructions/recalculate-all-closure-table
     */
    instructionsRecalculateAllClosureTable: (params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/instructions/recalculate-all-closure-table`,
        method: "POST",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsRecalculateAllStatuses
     * @request POST:/api/instructions/recalculate-all-statuses
     */
    instructionsRecalculateAllStatuses: (params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/instructions/recalculate-all-statuses`,
        method: "POST",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsRecalculateStatusesForInWorkAndDeadline
     * @request POST:/api/instructions/recalculate-statuses-for-in-work-and-deadline
     */
    instructionsRecalculateStatusesForInWorkAndDeadline: (params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/instructions/recalculate-statuses-for-in-work-and-deadline`,
        method: "POST",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Instructions
     * @name InstructionsGenerateInstructions
     * @request POST:/api/instructions/generate-instructions
     */
    instructionsGenerateInstructions: (data: GenerationRm, params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/instructions/generate-instructions`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),
  };
}
