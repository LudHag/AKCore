export interface AlbumEditModel {
  id: number;
  name: string;
  image: string;
  year: number;
  category: string;
  created: string;
  released: string;
  tracks: TrackEditModel[];
}

export interface TrackEditModel {
  created: string;
  fileName: string;
  id: number;
  name: string;
  number: number;
}
