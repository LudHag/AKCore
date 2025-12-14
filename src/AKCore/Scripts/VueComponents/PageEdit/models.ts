import { AlbumEditModel } from "../AlbumEdit/models";

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
  type: string;
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
