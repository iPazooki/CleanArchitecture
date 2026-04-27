import en from './locales/en.json';
import fa from './locales/fa.json';
import ar from './locales/ar.json';

export const dictionaries = {
  en,
  fa,
  ar,
};

export type Locale = keyof typeof dictionaries;
export const supportedLocales = Object.keys(dictionaries) as Locale[];

export const defaultLocale: Locale = 'en';

export const isRtl = (locale: Locale) => {
  return locale === 'fa' || locale === 'ar';
};

export function isLocale(value: string | null | undefined): value is Locale {
  return Boolean(value) && supportedLocales.includes(value as Locale);
}
