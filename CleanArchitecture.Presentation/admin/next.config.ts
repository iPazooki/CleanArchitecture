import type { NextConfig } from "next";
import { getEnvVars } from "@/config/env-vars";

const { API_BASE_URL } = getEnvVars();
const apiRoutePrefix = "/api/v1";
const normalizedApiBaseUrl = API_BASE_URL.trim().replace(/\/+$/, "");

function getRewriteDestination(): string | undefined {
  if (!normalizedApiBaseUrl) {
    return undefined;
  }

  const proxiedApiBaseUrl = normalizedApiBaseUrl.endsWith(apiRoutePrefix)
    ? normalizedApiBaseUrl
    : `${normalizedApiBaseUrl}${apiRoutePrefix}`;

  return `${proxiedApiBaseUrl}/:path*`;
}

const rewriteDestination = getRewriteDestination();

const nextConfig: NextConfig = {
  async rewrites() {
    if (!rewriteDestination) {
      return [];
    }

    return [
      {
        source: `${apiRoutePrefix}/:path*`,
        destination: rewriteDestination,
      },
    ];
  },
  webpack(config) {
    config.module.rules.push({
      test: /\.svg$/,
      use: ["@svgr/webpack"],
    });

    return config;
  },
  turbopack: {
    rules: {
      "*.svg": {
        loaders: ["@svgr/webpack"],
        as: "*.js",
      },
    },
  },
};

export default nextConfig;
