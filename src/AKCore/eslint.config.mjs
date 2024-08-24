import js from "@eslint/js";
import eslintPluginVue from "eslint-plugin-vue";
import ts from "typescript-eslint";

export default ts.config(
  js.configs.recommended,
  ...ts.configs.recommended,
  ...eslintPluginVue.configs["flat/recommended"],
  {
    files: ["*.vue", "**/*.vue", "*.ts", "**/*.ts"],
    languageOptions: {
      parserOptions: {
        parser: "@typescript-eslint/parser",
      },
    },

    rules: {
      "@typescript-eslint/ban-ts-comment": "off",
      "@typescript-eslint/no-empty-interface": "off",
      "vue/no-mutating-props": "off",
      "no-prototype-builtins": "off",
      "vue/multi-word-component-names": "off",
      "@typescript-eslint/no-non-null-asserted-optional-chain": "off",
      "vue/valid-v-model": "off",
      "vue/one-component-per-file": "off",
      "@typescript-eslint/no-explicit-any": "off",
      "vue/html-self-closing": "off",
      "vue/max-attributes-per-line": "off",
      "vue/singleline-html-element-content-newline": "off",
      "vue/html-indent": "off",
      "@typescript-eslint/no-non-null-assertion": "off",
      "vue/html-closing-bracket-newline": "off",
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
);
