import type { Metadata } from "next";
import type { ReactNode } from "react";
import AuthProvider from "@/context/AuthContext";
import QueryProvider from "@/context/QueryContext";
import { SidebarProvider } from "@/context/SidebarContext";
import { ThemeProvider } from "@/context/ThemeContext";
import "./globals.css";
import "flatpickr/dist/flatpickr.css";

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
    <html lang="en" suppressHydrationWarning>
      <body className="antialiased dark:bg-gray-900">
        <AuthProvider>
          <QueryProvider>
            <ThemeProvider>
              <SidebarProvider>{children}</SidebarProvider>
            </ThemeProvider>
          </QueryProvider>
        </AuthProvider>
      </body>
    </html>
  );
}
