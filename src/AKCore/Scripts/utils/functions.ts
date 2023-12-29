export const groupBy = (xs: any[], key: string) => {
  return xs.reduce((rv, x) => {
    (rv[x[key]] = rv[x[key]] || []).push(x);
    return rv;
  }, {});
};

export const nameCompare = (a: any, b: any) => {
  if (a.name < b.name) return -1;
  if (a.name > b.name) return 1;
  return 0;
};

export const fmtMSS = (s: any) => {
  if (isNaN(s)) {
    return "0:00";
  }
  s = Math.floor(s);
  return (s - (s %= 60)) / 60 + (9 < s ? ":" : ":0") + s;
};

const frmtNum = (num: number) => {
  return ("0" + num).slice(-2);
};

export const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  const dateStr = frmtNum(date.getMonth() + 1) + "-" + frmtNum(date.getDate());
  const strTime = frmtNum(date.getHours()) + ":" + frmtNum(date.getMinutes());
  return dateStr + " " + strTime;
};

export const formatDateNoTime = (dateString: string) => {
  const date = new Date(dateString);
  const dateStr = frmtNum(date.getMonth() + 1) + "-" + frmtNum(date.getDate());
  return dateStr;
};
