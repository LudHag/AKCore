import { Translation } from "./models";

export const upcomingTranslations: Record<string, Translation> = {
  "sign-up": { swedish: "Anmäl", english: "Sign up" },
  "signed-up": { swedish: "Anmäld", english: "Signed up" },
  "type-of-play": { swedish: "Speltyp", english: "Type of concert" },
  "fika-and-clean": {
    swedish: "Fika och städning",
    english: "Fika and cleaning",
  },
  "ical-link": { swedish: "iCal-länk", english: "iCal-link" },
  list: { swedish: "Lista", english: "List" },
  month: { swedish: "Månad", english: "Month" },
  "no-concerts": {
    swedish: "Vi har tyvärr inga spelningar inplanerade närmaste tiden",
    english: "Unfortunately we have no planned concerts in the upcoming weeks",
  },
};
