---
name: add-translations
description: >-
  Adds bilingual Swedish/English translations for AKCore user-facing surfaces
  via TranslationsService (backend) and Scripts/translations (frontend). Use when
  adding, updating, or wiring translation keys, localize/i18n/translate strings,
  TranslationDomains, translate(), language cookie, textEng, NameEng, or
  DescriptionEng. Translations are primarily meant for user facing and not
  admin facing surfaces.
---

# Add Translations

AKCore uses a **custom bilingual system** (Swedish default, English via `language=EN` cookie). There is **no** ASP.NET `IStringLocalizer`, `.resx`, or vue-i18n.

## Scope (read first)

**Translations are primarily meant for user facing and not admin facing surfaces.**

| Translate | Do not translate |
|-----------|------------------|
| Public CMS pages, header, login | Admin Vue apps (PageEdit, Users, AdminEvent, Statistics, MenuEdit, Media, …) |
| Member apps: Profile, Upcoming, Signup, Music, Mailbox, MembersList, Videos, Countdown | Admin chrome, labels, buttons, validation toasts in admin UIs |
| API messages shown on user-facing pages | Internal/admin-only service errors unless they surface to members |

Admin tools may **author** English content (`textEng`, `NameEng`, `DescriptionEng`) for the public site, but admin UI strings stay Swedish.

## Decide where the string lives

1. **Razor / controller / API message** → backend `TranslationsService`
2. **Vue member/public app** → frontend `Scripts/translations/*.ts`
3. **Same wording in both** → update **both** dictionaries (keys are not shared)
4. **CMS widget body / menu / event copy** → DB `*Eng` fields (editor-authored), not static dictionaries

## Backend (static UI strings)

**File:** `src/AKCore/Services/TranslationsService.cs`

1. Add to the matching domain dictionary (or add a new `TranslationDomains` enum value + dictionary entry + register in `translations`).
2. Keys: **PascalCase**. Values: `new Translation("Swedish", "English")`.
3. Use in Razor (already injected as `t` via `_ViewImports.cshtml`):

```razor
@t.Get(TranslationDomains.Common, "FirstName")
@if (t.IsEnglish()) { ... }
```

4. Use in controllers/services via injected `TranslationsService`:

```csharp
_translationsService.Get(TranslationDomains.Profile, "ProfileUpdated")
```

## Frontend (Vue member/public apps)

**Folder:** `src/AKCore/Scripts/translations/`

1. Add key to the domain file (e.g. `profile.ts`). Keys: **kebab-case** (or existing style in that file). Shape:

```ts
"my-key": { swedish: "Svensk text", english: "English text" },
```

2. New domain: create `mydomain.ts`, then register in `index.ts` (`TranslationDomain` union + `translations` map).
3. In the Vue component:

```ts
import { TranslationDomain, translate } from "@scripts/translations";

const t = (key: string, domain: TranslationDomain = "profile") => {
  return translate(domain, key);
};
```

```vue
{{ t("my-key") }}
{{ t("first-name", "common") }}
```

`translate()` throws if the key is missing — always add the dictionary entry before using it.

## CMS / menu / event content (not dictionary keys)

| Content | Where authored | Runtime |
|---------|----------------|---------|
| Widget HTML | Page editor → `textEng` (`TranslationEdit.vue`) | `PageService` swaps `Text` ← `TextEng` when English |
| Menu labels | Menu editor → `NameEng` | Menu partials pick `NameEng` when English |
| Event copy | Admin event form → `DescriptionEng` / `InternalDescriptionEng` | `EventService` picks English fields when English |

Do not put long CMS HTML into `TranslationsService` or TS domain files.

## Language switching

- Cookie name: `language`; English when value is `"EN"`; otherwise Swedish.
- Toggle in header sets cookie and reloads (`Scripts/general.ts`).
- Frontend: `isEnglish` from `@scripts/translations`. Backend: `TranslationsService.IsEnglish()`.

## Checklist

- [ ] Surface is **user-facing** (skip admin UI unless explicitly requested)
- [ ] Correct layer(s): C# and/or TS and/or `*Eng` DB field
- [ ] Key naming matches that layer (PascalCase vs kebab-case)
- [ ] Swedish + English both filled
- [ ] If duplicated across C#/TS, keep wording in sync
- [ ] Verify with `language=EN` cookie and Swedish fallback when English is empty (CMS/`*Eng` only)

## Anti-patterns

- Do not add vue-i18n, `.resx`, or ASP.NET localization middleware
- Do not translate admin UI strings by default
- Do not invent a shared key scheme between C# and TS — they stay separate
- Do not hardcode user-facing Swedish in Razor/Vue when a domain already exists; extend the dictionary instead
