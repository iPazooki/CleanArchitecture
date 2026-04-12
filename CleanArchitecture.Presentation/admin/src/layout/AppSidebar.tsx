"use client";

import React, {useEffect, useRef, useState, useMemo, useCallback} from "react";
import Link from "next/link";
import Image from "next/image";
import {usePathname} from "next/navigation";
import {useSidebar} from "@/context/SidebarContext";
import { useLanguage } from "@/context/LanguageContext";
import {
    ChevronDownIcon,
    GridIcon,
    HorizontaLDots,
    PageIcon,
    UserCircleIcon,
} from "../icons/index";
import SidebarWidget from "./SidebarWidget";

type NavItem = {
    name: string;
    icon: React.ReactNode;
    path?: string;
    subItems?: { name: string; path: string; pro?: boolean; new?: boolean }[];
};

const navItems: NavItem[] = [
    {
        icon: <GridIcon/>,
        name: "Dashboard",
        subItems: [{name: "ecommerce", path: "/", pro: false}],
    },
    {
        icon: <PageIcon/>,
        name: "Books",
        path: "/book/list",
    },
    {
        icon: <UserCircleIcon/>,
        name: "User Profile",
        path: "/profile",
    },
];

const AppSidebar: React.FC = () => {
    const {isExpanded, isMobileOpen, isHovered, setIsHovered} = useSidebar();
    const pathname = usePathname();
    const { t } = useLanguage();

    const [subMenuHeight, setSubMenuHeight] = useState<Record<string, number>>(
        {}
    );
    const subMenuRefs = useRef<Record<string, HTMLDivElement | null>>({});

    const isActive = useCallback((path: string) => path === pathname, [pathname]);

    type OpenSubmenu = {
        type: "main" | "others";
        index: number;
    } | null;

    const [manualSubmenuState, setManualSubmenuState] = useState<{
        pathname: string;
        submenu: OpenSubmenu;
    }>({
        pathname,
        submenu: null,
    });

    const routeMatchedSubmenu: OpenSubmenu = useMemo(() => {
        for (const [index, nav] of navItems.entries()) {
            if (nav.subItems?.some((subItem) => subItem.path === pathname)) {
                return {
                    type: "main",
                    index,
                };
            }
        }

        return null;
    }, [pathname]);

    const openSubmenu = useMemo(() => {
        return manualSubmenuState.pathname === pathname
            ? manualSubmenuState.submenu
            : routeMatchedSubmenu;
    }, [manualSubmenuState, pathname, routeMatchedSubmenu]);

    useEffect(() => {
        // Set the height of the submenu items when the submenu is opened
        if (openSubmenu !== null) {
            const key = `${openSubmenu.type}-${openSubmenu.index}`;
            if (subMenuRefs.current[key]) {
                const newHeight = subMenuRefs.current[key]?.scrollHeight || 0;
                setSubMenuHeight((prevHeights) => {
                    if (prevHeights[key] === newHeight) return prevHeights;
                    return {
                        ...prevHeights,
                        [key]: newHeight,
                    };
                });
            }
        }
    }, [openSubmenu]);

    const handleSubmenuToggle = (index: number, menuType: "main" | "others") => {
        setManualSubmenuState((previousState) => {
            const currentOpenSubmenu =
                previousState.pathname === pathname
                    ? previousState.submenu
                    : routeMatchedSubmenu;

            if (
                currentOpenSubmenu &&
                currentOpenSubmenu.type === menuType &&
                currentOpenSubmenu.index === index
            ) {
                return {
                    pathname,
                    submenu: null,
                };
            }

            return {
                pathname,
                submenu: {type: menuType, index},
            };
        });
    };

    const renderMenuItems = (
        navItems: NavItem[],
        menuType: "main" | "others"
    ) => (
        <ul className="flex flex-col gap-4">
            {navItems.map((nav, index) => (
                <li key={nav.name}>
                    {nav.subItems ? (
                        <button
                            onClick={() => handleSubmenuToggle(index, menuType)}
                            className={`menu-item group  ${
                                openSubmenu?.type === menuType && openSubmenu?.index === index
                                    ? "menu-item-active"
                                    : "menu-item-inactive"
                            } cursor-pointer ${
                                !isExpanded && !isHovered
                                    ? "lg:justify-center"
                                    : "lg:justify-start"
                            }`}
                        >
              <span
                  className={` ${
                      openSubmenu?.type === menuType && openSubmenu?.index === index
                          ? "menu-item-icon-active"
                          : "menu-item-icon-inactive"
                  }`}
              >
                {nav.icon}
              </span>
                            {(isExpanded || isHovered || isMobileOpen) && (
                                <span className={`menu-item-text`}>{t(nav.name as any) !== nav.name ? t(nav.name as any) : nav.name}</span>
                            )}
                            {(isExpanded || isHovered || isMobileOpen) && (
                                <ChevronDownIcon
                                    className={`rtl:mr-auto ltr:ml-auto w-5 h-5 transition-transform duration-200  ${
                                        openSubmenu?.type === menuType &&
                                        openSubmenu?.index === index
                                            ? "rotate-180 text-brand-500"
                                            : ""
                                    }`}
                                />
                            )}
                        </button>
                    ) : (
                        nav.path && (
                            <Link
                                href={nav.path}
                                className={`menu-item group ${
                                    isActive(nav.path) ? "menu-item-active" : "menu-item-inactive"
                                }`}
                            >
                <span
                    className={`${
                        isActive(nav.path)
                            ? "menu-item-icon-active"
                            : "menu-item-icon-inactive"
                    }`}
                >
                  {nav.icon}
                </span>
                                {(isExpanded || isHovered || isMobileOpen) && (
                                    <span className={`menu-item-text`}>{t(nav.name as any) !== nav.name ? t(nav.name as any) : nav.name}</span>
                                )}
                            </Link>
                        )
                    )}
                    {nav.subItems && (isExpanded || isHovered || isMobileOpen) && (
                        <div
                            ref={(el) => {
                                subMenuRefs.current[`${menuType}-${index}`] = el;
                            }}
                            className="overflow-hidden transition-all duration-300"
                            style={{
                                height:
                                    openSubmenu?.type === menuType && openSubmenu?.index === index
                                        ? `${subMenuHeight[`${menuType}-${index}`]}px`
                                        : "0px",
                            }}
                        >
                            <ul className="mt-2 space-y-1 rtl:mr-9 ltr:ml-9">
                                {nav.subItems.map((subItem) => (
                                    <li key={subItem.name}>
                                        <Link
                                            href={subItem.path}
                                            className={`menu-dropdown-item ${
                                                isActive(subItem.path)
                                                    ? "menu-dropdown-item-active"
                                                    : "menu-dropdown-item-inactive"
                                            }`}
                                        >
                                            {t(subItem.name as any) !== subItem.name ? t(subItem.name as any) : subItem.name}
                                            <span className="flex items-center gap-1 rtl:mr-auto ltr:ml-auto">
                        {subItem.new && (
                            <span
                                className={`rtl:mr-auto ltr:ml-auto ${
                                    isActive(subItem.path)
                                        ? "menu-dropdown-badge-active"
                                        : "menu-dropdown-badge-inactive"
                                } menu-dropdown-badge `}
                            >
                            new
                          </span>
                        )}
                                                {subItem.pro && (
                                                    <span
                                                        className={`rtl:mr-auto ltr:ml-auto ${
                                                            isActive(subItem.path)
                                                                ? "menu-dropdown-badge-active"
                                                                : "menu-dropdown-badge-inactive"
                                                        } menu-dropdown-badge `}
                                                    >
                            pro
                          </span>
                                                )}
                      </span>
                                        </Link>
                                    </li>
                                ))}
                            </ul>
                        </div>
                    )}
                </li>
            ))}
        </ul>
    );

    return (
        <aside
            className={`fixed mt-16 flex flex-col lg:mt-0 top-0 px-5 rtl:right-0 ltr:left-0 bg-white dark:bg-gray-900 dark:border-gray-800 text-gray-900 h-screen transition-all duration-300 ease-in-out z-50 border-x border-gray-200
        ${
                isExpanded || isMobileOpen
                    ? "w-[290px]"
                    : isHovered
                        ? "w-[290px]"
                        : "w-[90px]"
            }
        ${isMobileOpen ? "translate-x-0" : "max-lg:-translate-x-full max-lg:rtl:translate-x-full"}
        lg:translate-x-0`}
            onMouseEnter={() => !isExpanded && setIsHovered(true)}
            onMouseLeave={() => setIsHovered(false)}
        >
            <div
                className={`py-8 flex  ${
                    !isExpanded && !isHovered ? "lg:justify-center" : "justify-start"
                }`}
            >
                <Link href="/">
                    {isExpanded || isHovered || isMobileOpen ? (
                        <>
                            <Image
                                className="dark:hidden"
                                src="/images/logo/logo.svg"
                                alt="Logo"
                                width={150}
                                height={40}
                            />
                            <Image
                                className="hidden dark:block"
                                src="/images/logo/logo-dark.svg"
                                alt="Logo"
                                width={150}
                                height={40}
                            />
                        </>
                    ) : (
                        <Image
                            src="/images/logo/logo-icon.svg"
                            alt="Logo"
                            width={32}
                            height={32}
                        />
                    )}
                </Link>
            </div>
            <div className="flex flex-col overflow-y-auto duration-300 ease-linear no-scrollbar">
                <nav className="mb-6">
                    <div className="flex flex-col gap-4">
                        <div>
                            <h2
                                className={`mb-4 text-xs uppercase flex leading-[20px] text-gray-400 ${
                                    !isExpanded && !isHovered
                                        ? "lg:justify-center"
                                        : "justify-start"
                                }`}
                            >
                                {isExpanded || isHovered || isMobileOpen ? (
                                    t("Menu")
                                ) : (
                                    <HorizontaLDots/>
                                )}
                            </h2>
                            {renderMenuItems(navItems, "main")}
                        </div>
                    </div>
                </nav>
                {isExpanded || isHovered || isMobileOpen ? <SidebarWidget/> : null}
            </div>
        </aside>
    );
};

export default AppSidebar;
