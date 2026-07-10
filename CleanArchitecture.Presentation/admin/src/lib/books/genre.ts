const genreLabelByValue = {
  F: "Fiction",
  NF: "Non-Fiction",
  M: "Mystery",
} as const;

export type BookGenre = keyof typeof genreLabelByValue;

/** Non-empty tuple, as `z.enum` requires. */
export const genreValues = Object.keys(genreLabelByValue) as [BookGenre, ...BookGenre[]];

export const genreOptions = Object.entries(genreLabelByValue).map(([value, label]) => ({
  value: value as BookGenre,
  label,
}));

export function getGenreLabel(genre: string): string {
  return genreLabelByValue[genre as BookGenre] ?? genre;
}
