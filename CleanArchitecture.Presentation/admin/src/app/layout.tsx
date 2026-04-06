import type { Metadata } from "next";
import type { ReactNode } from "react";
import AuthProvider from "@/context/AuthContext";
import QueryProvider from "@/context/QueryContext";
import { SidebarProvider } from "@/context/SidebarContext";
import { ThemeProvider } from "@/context/ThemeContext";
import { LanguageProvider } from "@/context/LanguageContext";
import { Outfit } from "next/font/google";
import localFont from "next/font/local";
import "./globals.css";
import "flatpickr/dist/flatpickr.css";

const outfit = Outfit({
  subsets: ["latin"],
  variable: "--next-font-outfit",
  display: "swap",
});

const vazirmatn = localFont({
  src: "../../node_modules/vazirmatn/fonts/webfonts/Vazirmatn[wght].woff2",
  variable: "--next-font-vazirmatn",
  display: "swap",
});

export const metadata: Metadata = {
  title: {
    default: "Clean Architecture Admin",
    template: "%s | Clean Architecture Admin",
  },
  description: "Administrative dashboard for the Clean Architecture sample application.",
  robots: {
    index: false,
    follow: false,
  },
};

interface RootLayoutProps {
  children: ReactNode;
}

export default function RootLayout({ children }: Readonly<RootLayoutProps>) {
  return (
    <html
      lang="en"
      suppressHydrationWarning
      className={`${outfit.variable} ${vazirmatn.variable}`}
    >
      <body className="antialiased dark:bg-gray-900">
        <AuthProvider>
          <QueryProvider>
            <LanguageProvider>
              <ThemeProvider>
                <SidebarProvider>{children}</SidebarProvider>
              </ThemeProvider>
            </LanguageProvider>
          </QueryProvider>
        </AuthProvider>
      </body>
    </html>
  );
}
