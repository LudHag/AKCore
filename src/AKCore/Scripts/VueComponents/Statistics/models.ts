export type RequestsData = {
  amount: number;
  mobile: number;
  desktop: number;
  created: string;
};

export type RequestsPathsData = {
  path: string;
  items: RequestsData[];
};

export type RequestsResponse = {
  dates: string[];
  items: RequestsPathsData[];
};

export type RequestsRange = "day" | "week" | "month";

export type GigItem = {
  name: string;
  day: string;
  cantCome: number;
  canCome: number;
};

export type GigsResponse = {
  items: GigItem[];
};

export type GigsRange = "Month" | "Year";
