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

export function extractApiErrors(error: unknown): DomainError[] {
  if (isApiError(error)) {
    if (error.status === 401) {
      return [
        {
          code: "Unauthorized",
          message: "Your session has expired. Please sign in again.",
        },
      ];
    }

    if (typeof error.data === "string" && error.data.length > 0) {
      try {
        const parsedData = JSON.parse(error.data);
        if (isProblemDetails(parsedData)) {
          return extractErrors(parsedData);
        }
      } catch (e) {
        // Not a JSON string, fall through to return the raw string.
      }
      return [
        {
          code: "RequestError",
          message: error.data,
        },
      ];
    }

    if (isProblemDetails(error.data)) {
      return extractErrors(error.data);
    }
  }

  if (error instanceof Error && error.message) {
    return [
      {
        code: "UnexpectedError",
        message: error.message,
      },
    ];
  }

  return [
    {
      code: "NetworkError",
      message: "Network error occurred. Please try again.",
    },
  ];
}
