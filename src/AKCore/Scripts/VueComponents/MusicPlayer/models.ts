export interface Track {
  id: string;
  name: string;
  filepath: string;
  key: string;
}

export interface Album {
  category: string;
  image: string;
  name: string;
  tracks: { [key: number]: Track };
}
