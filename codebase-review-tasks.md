# Codebase review: proposed fix tasks

## 1) Typo fix task
**Issue**: In the admin icon exports, `CalenderIcon` and `calender-line.svg` use a misspelling of "Calendar".

**Proposed task**:
- Rename `CalenderIcon` to `CalendarIcon` in `src/icons/index.tsx`.
- Rename `calender-line.svg` to `calendar-line.svg` and update imports/usages.
- Run TypeScript checks/build to catch any missed references.

**Why this matters**: Consistent naming reduces confusion and improves discoverability in IDE autocomplete.

## 2) Bug fix task
**Issue**: The global exception handler writes `ProblemDetails` JSON but does not explicitly set `httpContext.Response.StatusCode`. This can result in `200 OK` responses containing error payloads, depending on middleware behavior.

**Proposed task**:
- Set `httpContext.Response.StatusCode` before each `WriteAsJsonAsync` call in `GlobalExceptionHandler` (at least for 401 and 500 branches).
- Add/adjust integration tests to assert both status code and payload shape for handled exceptions.

**Why this matters**: HTTP semantics should match the error payload to avoid client-side misinterpretation and retry bugs.

## 3) Comment/documentation discrepancy task
**Issue**: The root README refers to configuration files under `CleanArchitecture.Presentation/appsettings*.json`, but the actual API configuration files are under `CleanArchitecture.Presentation/API/appsettings*.json`.

**Proposed task**:
- Update README path references so they match the real folder structure.
- Validate all README file-path references with a quick scripted check.

**Why this matters**: Incorrect docs slow onboarding and lead to setup errors.

## 4) Test improvement task
**Issue**: Some integration tests only assert status codes and do not validate response bodies (e.g., created/get operations), which can miss regressions in contract shape and data.

**Proposed task**:
- In `BookIntegrationTests`, extend tests to assert response DTO/result payloads (e.g., created ID present, fetched title/genre exact match).
- Optionally assert RFC7807/problem-details fields for negative-path tests.

**Why this matters**: Stronger assertions catch API contract regressions earlier.
