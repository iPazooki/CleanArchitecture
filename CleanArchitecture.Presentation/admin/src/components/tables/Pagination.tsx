import Link from "next/link";

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  /** Maps a page number to its href, so the caller owns the URL shape. */
  buildHref: (page: number) => string;
  previousLabel: string;
  nextLabel: string;
}

const windowSize = 3;

/**
 * Pages are links, not callbacks: the current page lives in the URL, so it
 * survives a refresh and can be shared.
 */
function pageWindow(currentPage: number, totalPages: number): number[] {
  const start = Math.max(1, Math.min(currentPage - 1, totalPages - windowSize + 1));
  const length = Math.min(windowSize, totalPages);

  return Array.from({ length }, (_, index) => start + index);
}

const stepClassName =
  "flex h-10 items-center justify-center rounded-lg border border-gray-300 bg-white px-3.5 py-2.5 text-sm text-gray-700 shadow-theme-xs hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-white/[0.03]";
const disabledStepClassName =
  "pointer-events-none flex h-10 items-center justify-center rounded-lg border border-gray-300 bg-white px-3.5 py-2.5 text-sm text-gray-700 opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400";

export default function Pagination({
  currentPage,
  totalPages,
  buildHref,
  previousLabel,
  nextLabel,
}: PaginationProps) {
  if (totalPages <= 1) {
    return null;
  }

  const pages = pageWindow(currentPage, totalPages);
  const hasPrevious = currentPage > 1;
  const hasNext = currentPage < totalPages;

  return (
    <nav className="flex items-center" aria-label="Pagination">
      {hasPrevious ? (
        <Link href={buildHref(currentPage - 1)} rel="prev" className={`mr-2.5 ${stepClassName}`}>
          {previousLabel}
        </Link>
      ) : (
        <span aria-disabled className={`mr-2.5 ${disabledStepClassName}`}>
          {previousLabel}
        </span>
      )}

      <div className="flex items-center gap-2">
        {pages[0] > 1 ? <span className="px-2">...</span> : null}
        {pages.map((page) => (
          <Link
            key={page}
            href={buildHref(page)}
            aria-current={page === currentPage ? "page" : undefined}
            className={`flex h-10 w-10 items-center justify-center rounded-lg text-sm font-medium hover:bg-blue-500/[0.08] hover:text-brand-500 dark:hover:text-brand-500 ${
              page === currentPage
                ? "bg-brand-500 text-white"
                : "text-gray-700 dark:text-gray-400"
            }`}
          >
            {page}
          </Link>
        ))}
        {pages[pages.length - 1] < totalPages ? <span className="px-2">...</span> : null}
      </div>

      {hasNext ? (
        <Link href={buildHref(currentPage + 1)} rel="next" className={`ml-2.5 ${stepClassName}`}>
          {nextLabel}
        </Link>
      ) : (
        <span aria-disabled className={`ml-2.5 ${disabledStepClassName}`}>
          {nextLabel}
        </span>
      )}
    </nav>
  );
}
