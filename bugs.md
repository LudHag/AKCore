1. Image size is silently dropped on save (resizableImageExtension.ts) — real regression

In renderHTML, containerStyle is read and used in the early-return guard, but it's never applied to the inner div. containerAttrs stays empty:


resizableImageExtension.ts
Lines 42-73
const containerStyle = node.attrs.containerStyle as string | null;
...
if (!containerStyle && !wrapperStyle) {
  return innerContent;
}
const wrapperAttrs: Record<string, string> = {};
const containerAttrs: Record<string, string> = {};
if (wrapperStyle) {
  wrapperAttrs.style = wrapperStyle;
}
return ["div", wrapperAttrs, ["div", containerAttrs, innerContent]] as DOMOutputSpec;
The resize node view stores the image width in containerStyle (confirmed in the package: StyleManager.getContainerStyle(inline, \${width}px`)). Because containerAttrs.styleis never set, the width is lost the moment content is serialized viagetHTML()`. So a user resizes an image, it looks right while editing, but the saved HTML has no width. Fix:

if (containerStyle) {
  containerAttrs.style = containerStyle;
}
2. Stale link-modal state when linking an image (TextEditToolbar.vue)

setLink() for the image branch sets linkTarget, linkUrl, and opens the modal, but never resets linkShowTextField / linkInitialText:


TextEditToolbar.vue
Lines 336-345
  if (props.editor.isActive("imageResize")) {
    const previousUrl = props.editor.getAttributes("imageResize").href as
      | string
      | undefined;
    linkTarget.value = "image";
    linkUrl.value = previousUrl || "https://";
    showLinkModal.value = true;
    return;
  }
If the user previously edited a caret-position text link (linkShowTextField = true), then selects an image and clicks link, the modal will still render the "Länktext" field for the image. Reset linkShowTextField.value = false (and optionally linkInitialText.value = "") in the image branch.

3. insertContent builds HTML from unescaped strings (TextEditToolbar.vue)

Both applyLink and pickFile interpolate values straight into an HTML string:


TextEditToolbar.vue
Lines 406-406
      .insertContent(`<a href="${url}">${displayText}</a>`)
Tiptap re-parses through the schema so injected <script> won't persist, and LinkModal validates the URL — but displayText (and the filename in pickFile) are unescaped. A link text containing < / > / & will parse incorrectly, and it's a fragile pattern. Prefer the structured form, e.g. insertContent({ type: 'text', text: displayText, marks: [{ type: 'link', attrs: { href: url } }] }), which avoids string-building entirely. Worth noting given the repo rule to treat input as untrusted.

