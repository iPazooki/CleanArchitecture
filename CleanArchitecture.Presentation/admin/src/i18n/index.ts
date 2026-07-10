import en from './locales/en.json';
import fa from './locales/fa.json';
import ar from './locales/ar.json';

export const dictionaries = {
  en,
  fa,
  ar,
};

export type Locale = keyof typeof dictionaries;
export type TranslationKey = keyof typeof dictionaries['en'];
export const supportedLocales = Object.keys(dictionaries) as Locale[];

export const defaultLocale: Locale = 'en';

export const isRtl = (locale: Locale) => {
  return locale === 'fa' || locale === 'ar';
};

export function isLocale(value: string | null | undefined): value is Locale {
  return Boolean(value) && supportedLocales.includes(value as Locale);
}

/**
 * Error messages reach the UI from two sources: our zod schemas, which carry
 * translation keys, and the backend, which carries already-worded prose.
 */
export function isTranslationKey(value: string): value is TranslationKey {
  return value in dictionaries.en;
}
