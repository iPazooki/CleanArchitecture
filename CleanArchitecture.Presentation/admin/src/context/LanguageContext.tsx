"use client";

import React, { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import { Locale, defaultLocale, isRtl, dictionaries } from "@/i18n";

type LanguageContextType = {
  locale: Locale;
  setLocale: (locale: Locale) => void;
  t: (key: keyof typeof dictionaries["en"]) => string;
  isRtl: boolean;
};

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

export const LanguageProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [locale, setLocaleState] = useState<Locale>(defaultLocale);

  useEffect(() => {
    const savedLocale = localStorage.getItem("locale") as Locale;
    if (savedLocale && dictionaries[savedLocale]) {
      setLocaleState(savedLocale);
    }
  }, []);

  const setLocale = useCallback((newLocale: Locale) => {
    setLocaleState(newLocale);
    localStorage.setItem("locale", newLocale);
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
