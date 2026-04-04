const genreLabelByValue = {
  F: "Fiction",
  NF: "Non-Fiction",
  M: "Mystery",
} as const;

export type BookGenre = keyof typeof genreLabelByValue;

export const genreOptions = Object.entries(genreLabelByValue).map(([value, label]) => ({
  value: value as BookGenre,
  label,
}));

export function getGenreLabel(genre: string): string {
  return genreLabelByValue[genre as BookGenre] ?? genre;
}
