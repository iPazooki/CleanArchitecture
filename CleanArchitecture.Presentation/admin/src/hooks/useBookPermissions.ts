"use client";

import { useSession } from "next-auth/react";
import { hasRole } from "@/lib/auth/permissions";

export function useBookPermissions() {
  const { data: session } = useSession();
  const roles = session?.user?.roles;

  return {
    canCreate: hasRole(roles, "create"),
    canEdit: hasRole(roles, "edit"),
    canDelete: hasRole(roles, "delete"),
  };
}
