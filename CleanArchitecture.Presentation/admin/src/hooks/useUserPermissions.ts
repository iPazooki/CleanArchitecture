"use client";

import { useCallback } from "react";
import { useSession } from "next-auth/react";
import { hasRole, type BookRole } from "@/lib/auth/permissions";

/**
 * Role strings come from the IdP, so they stay unnamespaced verbs. `has` is the
 * extension point; the booleans cover today's callers.
 */
export function useUserPermissions() {
  const { data: session } = useSession();
  const roles = session?.user?.roles;

  const has = useCallback((role: BookRole) => hasRole(roles, role), [roles]);

  return {
    has,
    canCreate: has("create"),
    canEdit: has("edit"),
    canDelete: has("delete"),
  };
}
