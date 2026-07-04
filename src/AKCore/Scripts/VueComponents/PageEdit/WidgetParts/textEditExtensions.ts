import { Extension } from "@tiptap/core";

export const CustomClass = Extension.create({
  name: "customClass",

  addGlobalAttributes() {
    return [
      {
        types: ["paragraph", "heading"],
        attributes: {
          class: {
            default: null,
            parseHTML: (element) => element.getAttribute("class"),
            renderHTML: (attributes) => {
              if (!attributes.class) {
                return {};
              }
              return { class: attributes.class };
            },
          },
        },
      },
    ];
  },
});

export type TextStyleOption = {
  label: string;
  block: "paragraph" | "heading";
  level?: 1 | 2;
  className?: string | null;
};

export const TEXT_STYLE_OPTIONS: TextStyleOption[] = [
  { label: "Vanlig text", block: "paragraph" },
  { label: "Stor text", block: "paragraph", className: "big" },
  { label: "Rubrik 1", block: "heading", level: 1 },
  { label: "Rubrik 2", block: "heading", level: 2 },
  { label: "Infobox", block: "paragraph", className: "infobox" },
  { label: "3-delskolumn", block: "paragraph", className: "col-sm-4" },
  { label: "Block med marginal", block: "paragraph", className: "col-xs-12" },
];
