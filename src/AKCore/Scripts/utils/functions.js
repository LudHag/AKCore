export const groupBy = (xs, key) => {
  return xs.reduce((rv, x) => {
    (rv[x[key]] = rv[x[key]] || []).push(x);
    return rv;
  }, {});
};

export const nameCompare = (a, b) => {
  if (a.name < b.name) return -1;
  if (a.name > b.name) return 1;
  return 0;
};

export const fmtMSS = (s) => {
  if (isNaN(s)) {
    return "0:00";
  }
  s = Math.floor(s);
  return (s - (s %= 60)) / 60 + (9 < s ? ":" : ":0") + s;
};

const frmtNum = (num) => {
  return ("0" + num).slice(-2);
};

export const formatDate = (dateString) => {
  const date = new Date(dateString);
  const dateStr = frmtNum(date.getMonth() + 1) + "-" + frmtNum(date.getDate());
  const strTime = frmtNum(date.getHours()) + ":" + frmtNum(date.getMinutes());
  return dateStr + " " + strTime;
};

export const formatDateNoTime = (dateString) => {
  const date = new Date(dateString);
  const dateStr = frmtNum(date.getMonth() + 1) + "-" + frmtNum(date.getDate());
  return dateStr;
};
