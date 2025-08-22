export interface User {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string;
  instrument: string;
  otherInstruments: string[];
  roles: string[];
  posts: string[];
  slavPoster: string[];
  medal: string;
  givenMedal: string;
  hasKey: boolean;
  active: boolean;
  password: string;
  lastSignedIn: string;
  foodPreference: string | null;
}

export interface UpdateInfo {
  userName: string;
  prop: string;
  value: any;
}

export type SortUser = "name" | "userName" | "roles" | "lastSignedIn";
