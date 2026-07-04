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

function findAnchorAncestor(element: HTMLElement): HTMLAnchorElement | null {
  let el: HTMLElement | null = element.parentElement;
  let depth = 0;
  while (el && depth < 4) {
    if (el.tagName === "A") {
      return el as HTMLAnchorElement;
    }
    el = el.parentElement;
    depth++;
  }
  return null;
}

export const ResizableImage = ImageResize.extend({
  renderHTML({ node, HTMLAttributes }) {
    const containerStyle = node.attrs.containerStyle as string | null;
    const wrapperStyle = node.attrs.wrapperStyle as string | null;
    const href = node.attrs.href as string | null;
    const target = node.attrs.target as string | null;
    const rel = node.attrs.rel as string | null;

    const img: DOMOutputSpec = [
      "img",
      mergeAttributes(this.options.HTMLAttributes, HTMLAttributes),
    ];

    const anchorAttrs: Record<string, string> = {};
    if (href) anchorAttrs.href = href;
    if (target) anchorAttrs.target = target;
    if (rel) anchorAttrs.rel = rel;

    const innerContent: DOMOutputSpec = href
      ? (["a", anchorAttrs, img] as DOMOutputSpec)
      : img;

    if (!containerStyle && !wrapperStyle) {
      return innerContent;
    }

    const wrapperAttrs: Record<string, string> = {};
    const containerAttrs: Record<string, string> = {};

    if (wrapperStyle) {
      wrapperAttrs.style = wrapperStyle;
    }

    if (containerStyle) {
      containerAttrs.style = containerStyle;
    }

    return ["div", wrapperAttrs, ["div", containerAttrs, innerContent]] as DOMOutputSpec;
  },

  addAttributes() {
    return {
      ...this.parent?.(),
      href: {
        default: null,
        parseHTML: (element: HTMLElement) => {
          return findAnchorAncestor(element)?.getAttribute("href") ?? null;
        },
        renderHTML: () => ({}),
      },
      target: {
        default: null,
        parseHTML: (element: HTMLElement) => {
          return findAnchorAncestor(element)?.getAttribute("target") ?? null;
        },
        renderHTML: () => ({}),
      },
      rel: {
        default: null,
        parseHTML: (element: HTMLElement) => {
          return findAnchorAncestor(element)?.getAttribute("rel") ?? null;
        },
        renderHTML: () => ({}),
      },
      containerStyle: {
        default: null,
        parseHTML: (element: HTMLElement) => {
          // When img is wrapped in <a>, look at the anchor's parent for container style
          const parent =
            element.parentElement?.tagName === "A"
              ? element.parentElement.parentElement
              : element.parentElement;

          const fromParent = readLegacyOrParentStyle(
            element,
            "containerstyle",
            parent,
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
          // When img is wrapped in <a>, container and wrapper divs are one level higher
          const anchorParent =
            element.parentElement?.tagName === "A"
              ? element.parentElement
              : null;

          const container = anchorParent
            ? anchorParent.parentElement
            : element.parentElement;
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
