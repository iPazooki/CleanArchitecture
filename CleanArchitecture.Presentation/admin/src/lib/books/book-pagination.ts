import { z } from "zod";

export const defaultBookPageSize = 10;
export const maxBookPageSize = 100;

export const bookListRoute = "/book/list";

/**
 * Shared by the server page (parsing `searchParams`) and the client container
 * (rebuilding hrefs), so both agree on what a page link looks like.
 *
 * A junk `?page=` is a bookmark that rotted, not an error worth showing.
 */
export const bookListSearchParamsSchema = z.object({
  page: z.coerce.number().int().min(1).catch(1),
  pageSize: z.coerce.number().int().min(1).max(maxBookPageSize).catch(defaultBookPageSize),
});

export type BookListSearchParams = z.infer<typeof bookListSearchParamsSchema>;

export function buildBookListHref({ page, pageSize }: BookListSearchParams): string {
  const params = new URLSearchParams({ page: String(page) });

  if (pageSize !== defaultBookPageSize) {
    params.set("pageSize", String(pageSize));
  }

  return `${bookListRoute}?${params.toString()}`;
}
