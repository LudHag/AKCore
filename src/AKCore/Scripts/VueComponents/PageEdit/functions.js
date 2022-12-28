import { EventBus } from "../../utils/eventbus";

export const tinyMceOpts = (imageCallback, fileCallback) => ({
  selector: ".widget-area .mce-content",
  theme: "modern",
  plugins: [
    "advlist link image imagetools lists charmap print hr anchor spellchecker searchreplace wordcount code fullscreen media nonbreaking",
    "table contextmenu directionality emoticons template textcolor paste textcolor colorpicker textpattern",
  ],
  toolbar1:
    "bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | styleselect code fullscreen",
  toolbar2:
    "searchreplace bullist | undo redo | link unlink image | hr removeformat | charmap table",
  table_appearance_options: false,
  menubar: false,
  elementpath: true,
  convert_urls: false,
  style_formats: [
    { title: "Vanlig text", block: "p" },
    { title: "Stor text", block: "p", classes: "big" },
    { title: "Rubrik 1", block: "h1" },
    { title: "Rubrik 2", block: "h2" },
    { title: "Infobox", selector: "p", classes: "infobox" },
    { title: "3-delskolumn", block: "p", classes: "col-sm-4" },
    { title: "Block med marginal", block: "p", classes: "col-xs-12" },
  ],
  toolbar_items_size: "small",
  height: "200",
  content_css: "/dist/main.css",
  body_class: "body-content",
  browser_spellcheck: true,
  setup: function (ed) {
    ed.on("change", function (e) {
      const elcontent = ed.getContent();
      EventBus.trigger("editor-updated", { id: ed.id, content: elcontent });
    });
  },
  file_browser_callback: function (field_name, url, type, win) {
    if (type === "image") {
      imageCallback($("#" + field_name));
    }
    if (type === "file") {
      fileCallback($("#" + field_name));
    }
  },
});

export const getHeader = (type) => {
  switch (type) {
    case "Text":
      return "Text-widget";
    case "Image":
      return "Bild-widget";
    case "Video":
      return "Video-widget";
    case "Music":
      return "Musik-widget";
    case "Join":
      return "Gå med-widget";
    case "Hire":
      return "Anlita oss-widget";
    case "MemberList":
      return "Adressregister-widget";
    case "PostList":
      return "Kamererspostlista-widget";
    case "HeaderText":
      return "Headertext-widget";
    case "ThreePuffs":
      return "Tre puffar-widget";
    case "MailBox":
      return "Anonym brevlåda";
  }
  return "Text-bild-widget";
};
