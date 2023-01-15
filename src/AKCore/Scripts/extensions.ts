// eslint-disable-next-line @typescript-eslint/no-unused-vars
interface Date {
  addDays(days: number): Date;
}

Date.prototype.addDays = function (days: number) {
  const dat = new Date(this.valueOf());
  dat.setDate(dat.getDate() + days);
  return dat;
};
