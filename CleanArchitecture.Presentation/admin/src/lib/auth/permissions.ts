export const BOOK_ROLES = ["create", "edit", "delete"] as const;

export type BookRole = (typeof BOOK_ROLES)[number];

export function hasRole(
  roles: readonly string[] | undefined,
  requiredRole: BookRole,
): boolean {
  if (!roles?.length) {
    return false;
  }

  return roles.includes(requiredRole);
}
