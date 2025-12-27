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
