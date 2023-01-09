export interface MenuEditModel {
  id: number;
  name: string;
  link: string;
  linkId: number;
  children: MenuEditModel[];
  posIndex: number;
  loggedIn: boolean;
  menuLoggedIn: boolean;
  menuBalett: boolean;
}
export interface PageEditModel {
  id: number;
  name: string;
  slug: string;
  metaDescription: string;
  widgetsJson: string;
  loggedIn: boolean;
  loggedOut: boolean;
  balettOnly: boolean;
  lastModified: string;
  revisions: any;
}
