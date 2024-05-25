import { commonTranslations } from "./common";
import { instrumentsTranslations } from "./instruments";
import { loginTranslations } from "./login";
import { mailboxTranslations } from "./mailbox";
import { memberlistTranslations } from "./memberlist";
import { Translation } from "./models";
import { musicTranslations } from "./music";
import { profileTranslations } from "./profile";
import { signupTranslations } from "./signup";
import { upcomingTranslations } from "./upcoming";

export type TranslationDomain =
  | "login"
  | "common"
  | "profile"
  | "memberlist"
  | "instruments"
  | "upcoming"
  | "music"
  | "mailbox"
  | "signup";

const translations: Record<TranslationDomain, Record<string, Translation>> = {
  login: loginTranslations,
  common: commonTranslations,
  profile: profileTranslations,
  memberlist: memberlistTranslations,
  instruments: instrumentsTranslations,
  upcoming: upcomingTranslations,
  music: musicTranslations,
  mailbox: mailboxTranslations,
  signup: signupTranslations,
};

const getCookie = (name: string) => {
  const value = "; " + document.cookie;
  const parts = value.split("; " + name + "=");
  if (parts.length === 2) {
    return parts.pop()?.split(";").shift();
  }
};

export const isEnglish = getCookie("language") === "EN";

export const translate = (domain: TranslationDomain, key: string) => {
  const translation = translations[domain][key];
  if (!translation) {
    throw new Error(`No translation found for ${domain}.${key}`);
  }
  return isEnglish ? translation?.english : translation?.swedish;
};
