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

export interface AlbumEditModel {
  id: number;
  name: string;
  image: string;
  year: number;
  category: string;
  created: string;
  released: string;
}

export interface WidgetEditModel {
  id: number;
  type: string;
  text?: string;
  image?: string;
  videos?: Array<{
    link: string;
    title: string;
  }>;
  albums: number[];
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
