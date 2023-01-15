module.exports = {
  root: true,
  parser: "vue-eslint-parser",
  parserOptions: {
    parser: "@typescript-eslint/parser",
  },
  extends: [
    "plugin:vue/strongly-recommended",
    "eslint:recommended",
    "@vue/typescript/recommended",
  ],
  plugins: ["@typescript-eslint"],
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
  },
  globals: {
    $: "readonly",
    JQuery: "readonly",
  },
};
