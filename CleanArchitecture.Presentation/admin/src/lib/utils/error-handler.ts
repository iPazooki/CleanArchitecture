import type { ProblemDetails } from "../api/model";
import { isApiError } from "@/lib/utils/api-error";

export interface DomainError {
  code: string;
  message: string;
}

type ProblemDetailsWithErrors = ProblemDetails & {
  errors?: Array<Partial<DomainError>>;
};

function isProblemDetails(value: unknown): value is ProblemDetails {
  if (typeof value !== "object" || value === null) {
    return false;
  }

  return ["detail", "errors", "instance", "status", "title", "type"].some(
    (property) => property in value,
  );
}

function getProblemDetailsErrors(problemDetails: ProblemDetails): Array<Partial<DomainError>> | undefined {
  const { errors } = problemDetails as ProblemDetailsWithErrors;
  return Array.isArray(errors) ? errors : undefined;
}

export function extractErrors(problemDetails: ProblemDetails | null | undefined): DomainError[] {
  if (!problemDetails) {
    return [];
  }

  const errors = getProblemDetailsErrors(problemDetails);
  if (errors) {
    return errors.map((error) => ({
      code: error.code || "Error",
      message: error.message || error.code || "An unknown error occurred",
    }));
  }

  return [
    {
      code: "Unknown",
      message: problemDetails.detail || problemDetails.title || "An error occurred",
    },
  ];
}

export function formatErrorMessages(errors: DomainError[]): string {
  return errors.map((error) => error.message).join(", ");
}

export function getFieldErrors<TFieldName extends string>(
  errors: DomainError[],
  fieldMap: Record<string, TFieldName>,
): Partial<Record<TFieldName, string>> {
  const fieldErrors: Partial<Record<TFieldName, string>> = {};

  errors.forEach((error) => {
    for (const [substring, fieldName] of Object.entries(fieldMap)) {
      if (error.code.includes(substring)) {
        fieldErrors[fieldName] = error.message;
        break;
      }
    }
  });

  return fieldErrors;
}

/** Reads the response body of a failed API call, whether parsed or still a string. */
function extractResponseErrors(data: unknown): DomainError[] | undefined {
  if (typeof data === "string" && data.length > 0) {
    try {
      const parsedData: unknown = JSON.parse(data);
      if (isProblemDetails(parsedData)) {
        return extractErrors(parsedData);
      }
    } catch {
      // Not JSON — surface the raw string.
    }

    return [{ code: "RequestError", message: data }];
  }

  if (isProblemDetails(data)) {
    return extractErrors(data);
  }

  return undefined;
}

/**
 * A 401 is not special-cased here: QueryProvider owns re-authentication, so this
 * only has to describe what went wrong.
 */
export function extractApiErrors(error: unknown): DomainError[] {
  if (isApiError(error)) {
    const responseErrors = extractResponseErrors(error.data);
    if (responseErrors) {
      return responseErrors;
    }
  }

  if (error instanceof Error && error.message) {
    return [{ code: "UnexpectedError", message: error.message }];
  }

  return [{ code: "NetworkError", message: "Network error occurred. Please try again." }];
}
