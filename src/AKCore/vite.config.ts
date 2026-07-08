import { defineConfig } from "vitest/config";
import { resolve } from "path";
import { appendFileSync } from "fs";
import vue from "@vitejs/plugin-vue";
import { playwright } from "@vitest/browser-playwright";
import { visualizer } from "rollup-plugin-visualizer";
import { entrypoints, manifestTransform } from "./vite.utils";

// #region agent log
const _dbgLog = (msg: object) => {
  const line = JSON.stringify({sessionId:'fc1cd3',...msg,timestamp:Date.now()});
  try { appendFileSync('debug-fc1cd3.log', line + '\n'); } catch (_e) { void _e; }
  process.stderr.write('[debug-fc1cd3] ' + line + '\n');
};
_dbgLog({location:'vite.config.ts:1',message:'config module loaded',hypothesisId:'H1',data:{VITEST:process.env.VITEST,CI:process.env.CI}});
const _dbgPlugin = () => ({
  name: 'debug-hmr-check',
  configResolved(cfg: {server?:{hmr?:unknown}}) {
    _dbgLog({location:'vite.config.ts:configResolved',message:'resolved server.hmr',hypothesisId:'H1',data:{serverHmr:cfg.server?.hmr,isVitest:!!process.env.VITEST}});
  },
});
// #endregion agent log

// https://vitejs.dev/config/
export default defineConfig({
  test: {
    projects: [
      {
        extends: true,
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
    hmr: process.env.VITEST ? true : { clientPort: 5173 },
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
    // #region agent log
    _dbgPlugin(),
    // #endregion agent log
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
