import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import BookTable from "@/components/book/BookTable";
import { getAuthOptions } from "@/app/api/auth/[...nextauth]/route";
import { Metadata } from "next";
import { getServerSession } from "next-auth";
import Link from "next/link";
import { hasRole } from "@/lib/auth/permissions";

export const metadata: Metadata = {
  title: "Books List | Clean Architecture Admin",
  description: "View and manage books",
};

export default async function BookListPage() {
  const session = await getServerSession(getAuthOptions());
  const canCreate = hasRole(session?.user?.roles, "create");

  return (
    <div>
      <PageBreadcrumb pageTitle="Books" />
      <div className="space-y-6">
        <div className="flex justify-end">
          {canCreate ? (
            <Link
              href="/book/edit"
              className="inline-flex items-center justify-center rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              Add New Book
            </Link>
          ) : null}
        </div>
        <ComponentCard title="All Books">
          <BookTable />
        </ComponentCard>
      </div>
    </div>
  );
}
