/**
 * RFC 7807 Problem Details representation returned by ASP.NET Core API error responses.
 */
export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: { [key: string]: string[] };
}
