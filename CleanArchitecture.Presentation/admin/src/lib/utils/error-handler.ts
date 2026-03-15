import { ProblemDetails } from "../api/model";
import { ApiError } from "@/lib/orval-fetch";

/**
 * Represents a structured error from ProblemDetails
 */
export interface DomainError {
  code: string;
  message: string;
}

/**
 * Extracts structured errors from ProblemDetails response
 */
export function extractErrors(problemDetails: ProblemDetails): DomainError[] {
  if (!problemDetails) {
    return [];
  }

  // Check if errors extension exists (our custom format)
  const extensions = problemDetails as any;
  if (extensions.errors && Array.isArray(extensions.errors)) {
    return extensions.errors as DomainError[];
  }

  // Fallback to generic error
  return [
    {
      code: "Unknown",
      message: problemDetails.detail || problemDetails.title || "An error occurred",
    },
  ];
}

/**
 * Formats errors for display
 */
export function formatErrorMessages(errors: DomainError[]): string {
  return errors.map((e) => e.message).join(", ");
}

/**
 * Gets field-specific errors for form validation
 */
export function getFieldErrors(
  errors: DomainError[],
  fieldMap: Record<string, string>,
): Record<string, string> {
  const fieldErrors: Record<string, string> = {};

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

/**
 * Extracts DomainError[] from an unknown error, handling ApiError with
 * ProblemDetails body for 400 responses.
 */
export function extractApiErrors(error: unknown): DomainError[] {
  if (error instanceof ApiError && error.status === 400) {
    return extractErrors(error.data as ProblemDetails);
  }
  return [
    {
      code: "NetworkError",
      message: "Network error occurred. Please try again.",
    },
  ];
}
