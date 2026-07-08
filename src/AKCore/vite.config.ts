import { defineConfig } from "vitest/config";
import { resolve } from "path";
import vue from "@vitejs/plugin-vue";
import { playwright } from "@vitest/browser-playwright";
import { visualizer } from "rollup-plugin-visualizer";
import { entrypoints, manifestTransform } from "./vite.utils";

export default defineConfig({
  test: {
    projects: [
      {
        extends: true,
        server: {
          hmr: false,
        },
        test: {
          name: "component-tests",
          include: ["test/components/**/*.test.ts"],
          setupFiles: ["vitest-browser-vue"],
          browser: {
            headless: true,
            enabled: true,
            provider: playwright(),
            instances: [{ browser: "chromium" }],
            viewport: { width: 1366, height: 768 },
          },
        },
      },
    ],
  },
  server: {
    port: 5173,
    hmr: { clientPort: 5173 },
  },
  resolve: {
    alias: {
      "@styles": resolve(__dirname, "Styles"),
      "@scripts": resolve(__dirname, "Scripts"),
      "@components": resolve(__dirname, "Scripts/VueComponents"),
      "@utils": resolve(__dirname, "Scripts/utils"),
      "@services": resolve(__dirname, "Scripts/services"),
      "@test": resolve(__dirname, "test"),
    },
  },
  plugins: [
    vue(),
    manifestTransform(),
    ...(process.env.ANALYZE
      ? [
          visualizer({
            open: true,
          }),
        ]
      : []),
  ],
  experimental: {
    renderBuiltUrl: (filename) => {
      return filename.replace("wwwroot", "");
    },
  },
  optimizeDeps: {
    include: ["vue"],
  },
  build: {
    manifest: "manifest.json",
    outDir: ".",
    emptyOutDir: false,

    rolldownOptions: {
      input: entrypoints.reduce(
        (acc, entrypoint) => {
          acc[entrypoint] = resolve(__dirname, `Scripts/${entrypoint}.ts`);
          return acc;
        },
        {} as Record<string, string>,
      ),
      output: {
        assetFileNames: "wwwroot/dist/[name]-[hash].[ext]",
        chunkFileNames: "wwwroot/dist/[name]-[hash].js",
        entryFileNames: "wwwroot/dist/[name]-[hash].js",
      },
    },
  },
});
