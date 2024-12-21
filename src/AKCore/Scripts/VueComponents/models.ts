export interface Member {
  name: string;
  email: string;
  phone: string;
  instrument: string;
}

export interface Video {
  link: string;
  title: string;
}

interface Uploadable {
  id: number;
  name: string;
  type: string;
  tag: string;
  created: string;
}

export interface Image extends Uploadable {}

export interface Document extends Uploadable {}
