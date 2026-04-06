import { getAuthOptions } from "@/app/api/auth/[...nextauth]/route";
import { Metadata } from "next";
import { getServerSession } from "next-auth";
import { hasRole } from "@/lib/auth/permissions";
import PageContent from "./PageContent";

export const metadata: Metadata = {
  title: "Books List | Clean Architecture Admin",
  description: "View and manage books",
};

export default async function BookListPage() {
  const session = await getServerSession(getAuthOptions());
  const canCreate = hasRole(session?.user?.roles, "create");

  return (
    <PageContent canCreate={canCreate} />
  );
}
