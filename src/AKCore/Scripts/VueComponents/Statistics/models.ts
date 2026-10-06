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

export type UsageDataPoint = {
  amount: number;
  created: string;
};

export type UsageTypeData = {
  type: string;
  items: UsageDataPoint[];
};

export type UsageResponse = {
  dates: string[];
  items: UsageTypeData[];
};

export type GigItem = {
  id: number;
  name: string;
  day: string;
  cantCome: number;
  canCome: number;
};

export type GigsResponse = {
  items: GigItem[];
};

export type GigsRange = "Month" | "Year" | "AllTime";
