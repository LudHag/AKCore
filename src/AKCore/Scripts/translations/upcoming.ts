import { Translation } from "./models";

export const upcomingTranslations: Record<string, Translation> = {
  "sign-up": { swedish: "Anmäl", english: "Sign up" },
  "signed-up": { swedish: "Anmäld", english: "Signed up" },
  "about-event": { swedish: "Om spelningen", english: "More info" },
  "type-of-play": { swedish: "Speltyp", english: "Type of concert" },
  "fika-and-clean": {
    swedish: "Fika och städning",
    english: "Fika and cleaning",
  },
  "ical-link": { swedish: "iCal-länk", english: "iCal-link" },
  list: { swedish: "Lista", english: "List" },
  month: { swedish: "Månad", english: "Month" },
  allFilter: { swedish: "Alla rep", english: "All rehearsals" },
  balletFilter: { swedish: "Balettrep", english: "Ballet rehearsals" },
  orchestraFilter: { swedish: "Orkesterrep", english: "Orchestra rehearsals" },
  "no-concerts": {
    swedish: "Vi har tyvärr inga spelningar inplanerade närmaste tiden",
    english: "Unfortunately we have no planned concerts in the upcoming weeks",
  },
  Rep: { swedish: "Rep", english: "Rehearsal" },
  Kårhusrep: {
    swedish: "Kårhusrep",
    english: "Student union building rehearsal",
    },
  Balettrep: {
      swedish: "Balettrep",
      english: "Ballet rehearsal",
    },
  Athenrep: { swedish: "Athenrep", english: "Athen rehearsal" },
  Samlingsrep: { swedish: "Gemensamt rep", english: "Collective rehearsal" },
  Fikarep: { swedish: "Fikarep", english: "Fika rehearsal" },
};
