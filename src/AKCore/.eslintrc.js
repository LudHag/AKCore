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
  },
  globals: {
    $: "readonly",
    JQuery: "readonly",
  },
};
