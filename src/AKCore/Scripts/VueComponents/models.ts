export interface Member {
  name: string;
  email: string;
  phone: string;
  instrument: string;
}

export interface ProfileData {
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  phone: string;
  instrument: string;
  otherInstruments: string[];
  medal: string | null;
  givenMedal: string | null;
  roles: string[];
  posts: string[];
}

export interface Video {
  link: string;
  title: string;
}
