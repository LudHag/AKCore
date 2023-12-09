export const getHeader = (type: string): string => {
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
