import { UpcomingEvent } from "./models";

export const eventIsRep = (event: UpcomingEvent) =>
    event.type === "Rep" ||
    event.type === "Kårhusrep" ||
    event.type === "Athenrep" ||
    event.type === "Fikarep" ||
    event.type == "Balettrep";
