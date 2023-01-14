import { AlbumEditModel } from "../AlbumEdit/models";

export interface PageRevisionEditModel {
  id: number;
  name: string;
  slug: string;
  widgets: WidgetEditModel[];
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
  image?: string;
  videos?: EditVideoModel[];
  albums: number[];
}

export interface EditVideoModel {
  link: string;
  title: string;
  index?: number;
}

export interface PageEditModel {
  name: string;
  slug: string;
  loggedIn: boolean;
  loggedOut: boolean;
  balettOnly: boolean;
  template: string;
  pageId: number;
  selectedRevision: number;
  lastModified: string;
  widgets: WidgetEditModel[];
  albums: AlbumEditModel[];
  revisions: PageRevisionEditModel[];
}
