"use client";

import {
  getGetApiV1BooksIdQueryKey,
  getGetApiV1BooksQueryKey,
  useGetApiV1Books,
  useGetApiV1BooksId,
} from "@/lib/api/books/books";
import type { BookResponse } from "@/lib/api/model";

/**
 * The seam between the app and the orval-generated client. Components depend on
 * this module, never on the generated symbols, so regenerating the client or
 * swapping transports stays contained here.
 */
export const bookQueryKeys = {
  list: () => getGetApiV1BooksQueryKey(),
  detail: (id: string) => getGetApiV1BooksIdQueryKey(id),
};

export interface BookPagination {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface BooksQueryParams {
  page: number;
  pageSize: number;
}

/** The OpenAPI spec types the paging fields as `number | string`. */
function toNumber(value: number | string | undefined, fallback: number): number {
  const parsed = typeof value === "string" ? Number.parseInt(value, 10) : value;
  return typeof parsed === "number" && Number.isFinite(parsed) ? parsed : fallback;
}

export function useBooks({ page, pageSize }: BooksQueryParams) {
  const query = useGetApiV1Books({ Page: page, PageSize: pageSize });
  const payload = query.data?.status === 200 ? query.data.data : undefined;

  const pagination: BookPagination = {
    page: toNumber(payload?.page, page),
    pageSize: toNumber(payload?.pageSize, pageSize),
    totalCount: toNumber(payload?.totalCount, 0),
    totalPages: toNumber(payload?.totalPages, 1),
  };

  return {
    books: payload?.items ?? [],
    pagination,
    isLoading: query.isLoading,
    error: query.error,
  };
}

export function useBook(id: string) {
  const query = useGetApiV1BooksId(id, { query: { enabled: Boolean(id) } });
  const book: BookResponse | undefined = query.data?.status === 200 ? query.data.data : undefined;

  return {
    book,
    isLoading: query.isLoading,
    error: query.error,
  };
}
