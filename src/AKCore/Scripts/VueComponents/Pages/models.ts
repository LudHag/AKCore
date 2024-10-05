export type PageEditModel = {
  id: number;
  name: string;
  slug: string;
  metaDescription: string;
  widgetsJson: string;
  loggedIn: boolean;
  loggedOut: boolean;
  balettOnly: boolean;
  lastModified: string;
};

export type PagesRequesponse = {
  pages: PageEditModel[];
};

export type SortType = "name" | "slug" | "loggedin" | "lastModified";
