export interface Track {
  id: string;
  name: string;
  filepath: string;
  key: string;
}

export interface Album {
  id: string;
  category: string;
  image: string;
  name: string;
  tracks: Track[];
}
