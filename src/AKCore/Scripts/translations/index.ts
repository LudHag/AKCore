import { commonTranslations } from "./common";
import { instrumentsTranslations } from "./instruments";
import { loginTranslations } from "./login";
import { memberlistTranslations } from "./memberlist";
import { Translation } from "./models";
import { profileTranslations } from "./profile";

export type TranslationDomain =
  | "login"
  | "common"
  | "profile"
  | "memberlist"
  | "instruments";

const translations: Record<TranslationDomain, Record<string, Translation>> = {
  login: loginTranslations,
  common: commonTranslations,
  profile: profileTranslations,
  memberlist: memberlistTranslations,
  instruments: instrumentsTranslations,
};

const getCookie = (name: string) => {
  const value = "; " + document.cookie;
  const parts = value.split("; " + name + "=");
  if (parts.length === 2) {
    return parts.pop()?.split(";").shift();
  }
};

const isEnglish = getCookie("language") === "EN";

export const translate = (domain: TranslationDomain, key: string) => {
  const translation = translations[domain][key];
  return isEnglish ? translation?.english : translation?.swedish;
};
