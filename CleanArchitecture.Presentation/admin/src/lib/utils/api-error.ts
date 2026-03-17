export class ApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly data: unknown,
    public readonly headers: Headers,
  ) {
    super(`API request failed with status ${status}`);
    this.name = "ApiError";
  }
}

export function isApiError(error: unknown): error is ApiError {
  return error instanceof ApiError;
}
