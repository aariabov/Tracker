import { Api as UsersApi } from "./api/UsersApi";
import { Api as AnalyticsApi } from "./api/AnalyticsApi";
import { Api as AuditApi } from "./api/AuditApi";
import { Api as InstructionsApi } from "./api/InstructionsApi";


export const apiClientUsers = new UsersApi<string>({
    securityWorker: accessToken =>
      accessToken ? { headers: { Authorization: `Bearer ${accessToken}` } } : {},
  });
  
export const apiClientAnalytics = new AnalyticsApi<string>({
  securityWorker: accessToken =>
    accessToken ? { headers: { Authorization: `Bearer ${accessToken}` } } : {},
});

export const apiClientAudit = new AuditApi<string>({
  securityWorker: accessToken =>
    accessToken ? { headers: { Authorization: `Bearer ${accessToken}` } } : {},
});

export const apiClientInstructions = new InstructionsApi<string>({
securityWorker: accessToken =>
  accessToken ? { headers: { Authorization: `Bearer ${accessToken}` } } : {},
});
