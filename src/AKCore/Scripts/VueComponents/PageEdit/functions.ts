import { WidgetType } from "./models";

const widgetHeaders: Record<WidgetType, string> = {
  Text: "Text-widget",
  TextImage: "Text-bild-widget",
  Image: "Bild-widget",
  Video: "Video-widget",
  Music: "Musik-widget",
  Join: "Gå med-widget",
  Hire: "Anlita oss-widget",
  MemberList: "Adressregister-widget",
  PostList: "Kamererspostlista-widget",
  HeaderText: "Headertext-widget",
  ThreePuffs: "Tre puffar-widget",
  MailBox: "Anonym brevlåda-widget",
  CountDown: "Nedräknare-widget",
  VideosHeader: "Video rubrik med sök-widget",
};

export const getHeader = (type: WidgetType): string => widgetHeaders[type];
