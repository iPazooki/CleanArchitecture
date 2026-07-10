import { getAuthOptions } from "@/lib/auth/auth-options";
import { Metadata } from "next";
import { getServerSession } from "next-auth";
import { hasRole } from "@/lib/auth/permissions";
import { bookListSearchParamsSchema } from "@/lib/books/book-pagination";
import PageContent from "./PageContent";

export const metadata: Metadata = {
  title: "Books List | Clean Architecture Admin",
  description: "View and manage books",
};

interface BookListPageProps {
  searchParams: Promise<Record<string, string | string[] | undefined>>;
}

export default async function BookListPage({ searchParams }: BookListPageProps) {
  const session = await getServerSession(getAuthOptions());
  const canCreate = hasRole(session?.user?.roles, "create");

  const { page, pageSize } = bookListSearchParamsSchema.parse(await searchParams);

  return <PageContent canCreate={canCreate} page={page} pageSize={pageSize} />;
}
