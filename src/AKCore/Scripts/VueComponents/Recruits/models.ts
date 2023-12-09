export interface Recruit {
  fname: string;
  lname: string;
  archived: boolean;
  created: Date;
  email: string;
  phone: string;
  instrument: string;
  other: string;
  id: number;
}

export interface RecruitUpdate {
  id: number;
  arch: boolean;
}
