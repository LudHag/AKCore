import { AlbumEditModel } from "../AlbumEdit/models";

export const WIDGET_TYPES = [
  "Text",
  "TextImage",
  "Image",
  "Video",
  "Music",
  "HeaderText",
  "MemberList",
  "PostList",
  "Join",
  "Hire",
  "MailBox",
  "CountDown",
  "ThreePuffs",
  "VideosHeader",
] as const;

export type WidgetType = (typeof WIDGET_TYPES)[number];

export interface PageRevisionEditModel {
  id: number;
  name: string;
  slug: string;
  widgets: WidgetEditModel[];
  metaDescription: string;
  loggedIn: boolean;
  loggedOut: boolean;
  balettOnly: boolean;
  modified: string;
  modifiedBy: string;
}

export interface WidgetEditModel {
  id: number;
  type: WidgetType;
  text?: string;
  textEng?: string;
  image?: string;
  imageAlt?: string;
  videos?: EditVideoModel[];
  targetDate?: string;
  targetTime?: string;
  albums: number[];
}

export interface EditVideoModel {
  link: string;
  title: string;
  index: number;
}

export interface PageEditModel {
  name: string;
  slug: string;
  loggedIn: boolean;
  loggedOut: boolean;
  balettOnly: boolean;
  template: string;
  pageId: number;
  metaDescription: string;
  selectedRevision: number;
  lastModified: string;
  widgets: WidgetEditModel[];
  albums: AlbumEditModel[];
  revisions: PageRevisionEditModel[];
}
