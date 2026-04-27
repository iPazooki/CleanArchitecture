"use client";

import React, { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import { Locale, defaultLocale, isLocale, isRtl, dictionaries } from "@/i18n";

const localeStorageKey = "locale";
const localeCookieName = "admin-locale";

type LanguageContextType = {
  locale: Locale;
  setLocale: (locale: Locale) => void;
  t: (key: keyof typeof dictionaries["en"]) => string;
  isRtl: boolean;
};

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

type LanguageProviderProps = {
  children: React.ReactNode;
  initialLocale?: Locale;
};

function readStoredLocale(): Locale | null {
  if (typeof window === "undefined") {
    return null;
  }

  const savedLocale = window.localStorage.getItem(localeStorageKey);
  return isLocale(savedLocale) ? savedLocale : null;
}

export const LanguageProvider: React.FC<LanguageProviderProps> = ({
  children,
  initialLocale = defaultLocale,
}) => {
  const [locale, setLocaleState] = useState<Locale>(() => readStoredLocale() ?? initialLocale);

  const setLocale = useCallback((newLocale: Locale) => {
    setLocaleState(newLocale);
    window.localStorage.setItem(localeStorageKey, newLocale);
    document.cookie = `${localeCookieName}=${newLocale}; path=/; max-age=31536000; samesite=lax`;
  }, []);

  const t = useCallback(
    (key: keyof typeof dictionaries["en"]) => {
      return dictionaries[locale][key] || dictionaries[defaultLocale][key] || key;
    },
    [locale],
  );

  const rtl = isRtl(locale);

  useEffect(() => {
    document.documentElement.dir = rtl ? "rtl" : "ltr";
    document.documentElement.lang = locale;
  }, [locale, rtl]);

  const value = useMemo(
    () => ({ locale, setLocale, t, isRtl: rtl }),
    [locale, setLocale, t, rtl],
  );

  return (
    <LanguageContext.Provider value={value}>
      {children}
    </LanguageContext.Provider>
  );
};

export const useLanguage = () => {
  const context = useContext(LanguageContext);
  if (!context) {
    throw new Error("useLanguage must be used within a LanguageProvider");
  }
  return context;
};
