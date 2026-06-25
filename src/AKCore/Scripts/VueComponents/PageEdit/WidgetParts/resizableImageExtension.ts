import { mergeAttributes } from "@tiptap/core";
import type { DOMOutputSpec } from "@tiptap/pm/model";
import ImageResize from "tiptap-extension-resize-image";

const DEFAULT_WRAPPER_STYLE = "display: flex; margin: 0;";

function readLegacyOrParentStyle(
  element: HTMLElement,
  legacyAttribute: string,
  parent: HTMLElement | null,
): string | null {
  const legacy = element.getAttribute(legacyAttribute);
  if (legacy) {
    return legacy;
  }

  if (parent?.tagName === "DIV") {
    const style = parent.getAttribute("style");
    if (style) {
      return style;
    }
  }

  return null;
}

export const ResizableImage = ImageResize.extend({
  renderHTML({ node, HTMLAttributes }) {
    const containerStyle = node.attrs.containerStyle as string | null;
    const wrapperStyle = node.attrs.wrapperStyle as string | null;

    const img: DOMOutputSpec = [
      "img",
      mergeAttributes(this.options.HTMLAttributes, HTMLAttributes),
    ];

    if (!containerStyle && !wrapperStyle) {
      return img;
    }

    const wrapperAttrs: Record<string, string> = {};
    const containerAttrs: Record<string, string> = {};

    if (wrapperStyle) {
      wrapperAttrs.style = wrapperStyle;
    }

    return ["div", wrapperAttrs, ["div", containerAttrs, img]] as DOMOutputSpec;
  },

  addAttributes() {
    return {
      ...this.parent?.(),
      containerStyle: {
        default: null,
        parseHTML: (element: HTMLElement) => {
          const fromParent = readLegacyOrParentStyle(
            element,
            "containerstyle",
            element.parentElement,
          );
          if (fromParent) {
            return fromParent;
          }

          const width = element.getAttribute("width");
          if (width) {
            return `width: ${width}px; height: auto;`;
          }

          return element.style.cssText || null;
        },
        renderHTML: () => ({}),
      },
      wrapperStyle: {
        default: DEFAULT_WRAPPER_STYLE,
        parseHTML: (element: HTMLElement) => {
          const container = element.parentElement;
          const wrapper = container?.parentElement ?? null;
          const fromParent = readLegacyOrParentStyle(
            element,
            "wrapperstyle",
            wrapper,
          );
          if (fromParent) {
            return fromParent;
          }

          return DEFAULT_WRAPPER_STYLE;
        },
        renderHTML: () => ({}),
      },
    };
  },
});
