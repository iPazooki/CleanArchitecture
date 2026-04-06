"use client";

import { useLanguage } from "@/context/LanguageContext";
import { Locale } from "@/i18n";

export const LanguageSwitcher = () => {
  const { locale, setLocale, t } = useLanguage();

  const handleLanguageChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setLocale(e.target.value as Locale);
  };

  return (
    <div className="flex items-center gap-2">
      <select
        value={locale}
        onChange={handleLanguageChange}
        className="block w-full rounded-md border-gray-300 py-1.5 text-gray-900 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm sm:leading-6 dark:bg-gray-800 dark:border-gray-700 dark:text-white"
      >
        <option value="en">English</option>
        <option value="fa">فارسی</option>
        <option value="ar">العربية</option>
      </select>
    </div>
  );
};
