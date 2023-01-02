export interface Track {
  id: string;
  name: string;
  filepath: string;
  key: string;
}

export interface Album {
  id: number;
  category: string;
  image: string;
  name: string;
  tracks: Track[];
}

export interface Albums {
  [key: string]: Album;
}
