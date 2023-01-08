import { Api } from "./api/Api";

export const apiClient = new Api<string>({
    securityWorker: accessToken =>
      accessToken ? { headers: { Authorization: `Bearer ${accessToken}` } } : {},
  });
