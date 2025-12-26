import js from "@eslint/js";
import eslintPluginVue from "eslint-plugin-vue";
import ts from "typescript-eslint";
import vueParser from "vue-eslint-parser";
import globals from "globals";
import prettierConfig from "eslint-config-prettier";

export default [
  js.configs.recommended,
  ...ts.configs.recommended,
  ...eslintPluginVue.configs["flat/recommended"],
  {
    files: ["*.vue", "**/*.vue"],
    languageOptions: {
      parser: vueParser,
      parserOptions: {
        parser: "@typescript-eslint/parser",
      },
      globals: {
        ...globals.browser,
      },
    },
  },
  {
    files: ["*.ts", "**/*.ts"],
    languageOptions: {
      parserOptions: {
        parser: "@typescript-eslint/parser",
      },
    },
  },
  {
    files: ["*.js", "**/*.js"],
    languageOptions: {
      globals: {
        ...globals.browser,
      },
    },
  },
  {
    files: ["*.ts", "**/*.ts", "*.vue", "**/*.vue"],
    rules: {
      "vue/no-mutating-props": "off",
      "no-prototype-builtins": "off",
      "vue/multi-word-component-names": "off",
      "vue/one-component-per-file": "off",
      "@typescript-eslint/no-explicit-any": "off",
      "@typescript-eslint/no-empty-object-type": "off",
      "vue/v-on-event-hyphenation": "off",
      "vue/no-v-html": "off",
      "vue/attributes-order": "off",
      "@typescript-eslint/no-unused-vars": [
        "error",
        {
          argsIgnorePattern: "^_",
          varsIgnorePattern: "^_",
          caughtErrorsIgnorePattern: "^_",
        },
      ],
    },
  },
  prettierConfig,
];
