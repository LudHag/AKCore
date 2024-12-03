import { Translation } from "./models";

export const commonTranslations: Record<string, Translation> = {
  close: { swedish: "Stäng", english: "Close" },
  password: { swedish: "Lösenord", english: "Password" },
  "user-name": { swedish: "Användarnamn", english: "User name" },
  coming: { swedish: "Kommer", english: "Coming" },
  "not-coming": { swedish: "Kommer inte", english: "Not coming" },
  "gather-in-hole": { swedish: "Samling i hålan", english: "Gather at hålan" },
  "gather-there": { swedish: "Samling på plats", english: "Gather there" },
  "concert-starts": { swedish: "Spelning startar", english: "Concert starts" },
  "at-rehersal-place": {swedish: 'Vid replokal', english: "At rehersal place"},
  "play-duration": { swedish: "Total spelningstid", english: "Play duration" },
  "sign-up-not-allowed": {
    swedish: "Anmälan till denna spelning är inte längre öppen. För mer info, kontakta styrelsen",
    english: "You can not sign up to this gig anymore. For more info, contact the board members",
  },
};
