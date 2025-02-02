import { RepFilterType } from "../models";
import { UpcomingEvent, UpcomingMonths, UpcomingYears } from "./models";

export const eventIsRep = (event: UpcomingEvent) =>
  event.type === "Rep" ||
  event.type === "Kårhusrep" ||
  event.type === "Athenrep" ||
  event.type === "Fikarep" ||
  event.type == "Balettrep" ||
  event.type == "Samlingsrep";

export const filterYears = (
  years: UpcomingYears,
  filter: RepFilterType,
): UpcomingYears => {
  const filteredYears: UpcomingYears = {};

  Object.entries(years).forEach(([year, yearData]) => {
    const filteredMonths: UpcomingMonths = {};

    Object.entries(yearData.months).forEach(([month, events]) => {
      const filteredEvents = events.filter((event) => {
        return (
          filter === "all" ||
          (filter === "ballet" && event.type !== "Rep") ||
          (filter === "orchestra" && event.type !== "Balettrep")
        );
      });

      if (filteredEvents.length > 0) {
        filteredMonths[month] = filteredEvents;
      }
    });

    if (Object.keys(filteredMonths).length > 0) {
      filteredYears[year] = { ...yearData, months: filteredMonths };
    }
  });

  return filteredYears;
};
