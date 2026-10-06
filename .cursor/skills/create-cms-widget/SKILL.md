---
name: create-cms-widget
description: >-
  Adds a new page widget type to the AKCore custom CMS (Vue page editor +
  Razor partial render + JSON Widget model). Use when creating, registering,
  wiring, or debugging a CMS widget, page widget type, AddWidget entry,
  Widget.vue editor switch, Widgets/*.cshtml partial, or WidgetsJson content.
---

# Create CMS widget

Widgets are page building blocks. The editor stores them as JSON (`WidgetsJson`); the public page renders each widget via a Razor partial named after `widget.Type`.

**Critical rule:** `Type` string must match everywhere — PascalCase, exact same spelling as the `.cshtml` file name (without extension). Example: type `"CountDown"` → `Views/Page/Widgets/CountDown.cshtml`.

## Checklist

Copy and track:

```
New widget: <TypeName>
- [ ] 1. Add type to WIDGET_TYPES in models.ts (+ fields on Widget/WidgetEditModel if needed)
- [ ] 2. Header in functions.ts widgetHeaders (Record forces completeness)
- [ ] 3. Editor Vue: Scripts/VueComponents/PageEdit/Widgets/<TypeName>.vue
- [ ] 4. Export: Widgets/widgets.ts
- [ ] 5. Mount editor: Widget.vue (import + v-if)
- [ ] 6. Add menu: AddWidget.vue (normal or special)
- [ ] 7. Render: Views/Page/Widgets/<TypeName>.cshtml
- [ ] 8. Optional interactive mount: vueapps.ts (+ frontend Vue app)
- [ ] 9. Optional ViewComponent / API / TranslationsService / SCSS
- [ ] 10. macrosWithNoTranslate if editor has no TextEng
```

No EF migration is needed for a new widget *type* — widgets live in JSON. Migrate only if the widget needs new DB tables/entities (e.g. MailBox items).

## 1. Shared data model

First register the type string in `WIDGET_TYPES` / `WidgetType` in `src/AKCore/Scripts/VueComponents/PageEdit/models.ts`. Then reuse fields on the shared `Widget` / `WidgetEditModel` when possible (`Text`, `TextEng`, `Image`, `ImageAlt`, `Videos`, `Albums`, `TargetDate`, `TargetTime`, …).

**C#** — `src/AKCore/DataModel/AkWidgets.cs`:

```csharp
public class Widget
{
    // add new serializable properties here when existing ones are insufficient
}
```

**TS** — `src/AKCore/Scripts/VueComponents/PageEdit/models.ts` (`WidgetEditModel`):
mirror any new C# properties (camelCase).

English content: editors store Swedish in `text` and English in `textEng`. `PageService.GetRenderModel` swaps `Text` ← `TextEng` when the language cookie is `EN`.

## 2. Editor component

Create `src/AKCore/Scripts/VueComponents/PageEdit/Widgets/<TypeName>.vue`.

Reuse existing parts when possible:
- `WidgetParts/TextEdit.vue` — rich text → `text`
- `WidgetParts/ImageEdit.vue` — image URL + alt
- `WidgetParts/TranslationEdit.vue` — English `textEng` UI

Patterns:
- **Content widget** (Text, TextImage, HeaderText): text/image editors + `TranslationEdit`, accept `translate: boolean`.
- **Config-only** (CountDown, Music, Video): custom fields; translation only if there is `text`.
- **No config** (MemberList, PostList, MailBox): empty template is fine. Still add to `AddWidget.vue` + render partial. Wiring an empty editor in `Widget.vue` is optional.

Emit updates with `update:modelValue` like sibling widgets. Keep props typed with `WidgetEditModel`.

## 3. Register in barrel + editor switch

**Export** — `Widgets/widgets.ts`: import and re-export the component.

**Switch** — `Widget.vue` (easy to forget):
1. Import from `./Widgets/widgets`
2. Add `v-if="modelValue.type === '<TypeName>'"` block
3. Pass `:model-value`, `@update:modelValue`, and extras (`:translate`, `:albums`) as needed

`PageEdit.vue` already creates widgets as `{ id, type, albums: [], text: "" }` — usually no change unless defaults must differ.

## 4. Header + add menu

**Label** — `functions.ts` `widgetHeaders`: add an entry for the new `WidgetType`. `Record<WidgetType, string>` fails compile if a type is missing.

**Menu** — `AddWidget.vue`:
- **Normal** (`dropdown-normal`): generic content (Text, Image, Video, …) — often glyphicon-only.
- **Special** (`dropdown-special`): feature widgets (MemberList, Join, CountDown, …) — Swedish label text.

`click('<TypeName>')` must use a value from `WIDGET_TYPES`.

## 5. Public render partial

Create `src/AKCore/Views/Page/Widgets/<TypeName>.cshtml` with `@model AKCore.DataModel.Widget`.

Rendered from `Views/Page/Index.cshtml`:

```cshtml
<partial name="Widgets/@w.Type" model="w"/>
```

A missing or mistyped partial breaks the page for that widget.

Notes:
- `@Html.Raw(Model.Text)` is the established pattern for editor HTML — do not introduce XSS vectors beyond that pattern; treat other fields as plain/escaped.
- Images: `@settings.CDN@Model.Image` (injected via `_ViewImports`).
- Static UI strings: `t.Get(TranslationDomains.Widgets, "Key")` and add keys in `TranslationsService`.
- Data-heavy widgets may `@await Component.InvokeAsync("...")` (see MemberList, Music/Albums, PostList).

## 6. Interactive frontend (optional)

If the partial only mounts a Vue root (e.g. `#countdown`, `#mailbox`, `.videos-app`):

1. Build the public app under `Scripts/VueComponents/...`
2. Mount it in `Scripts/vueapps.ts` when the DOM node exists
3. Pass data via inline `<script>` globals in the cshtml (existing pattern) or props
4. Client copy: `Scripts/translations/<name>.ts` when the app is bilingual

## 7. Translation / no-translate edge cases

- Widgets with `text` get an "Översätt" button in `Widget.vue` unless listed in `macrosWithNoTranslate`.
- Add the type to `macrosWithNoTranslate` when there is no meaningful `textEng` (Video is the current example).
- Form labels and fixed UI strings belong in `TranslationsService` (`TranslationDomains.Widgets` / Common), not in widget JSON.

## Architecture (quick)

```
AddWidget → PageEdit.widgets[] → save JSON (WidgetsJson)
                                      ↓
                              PageService.GetRenderModel
                                      ↓
                         Index.cshtml → Widgets/{Type}.cshtml
                                      ↓ (optional)
                                 vueapps.ts mounts
```

## Reference examples in repo

| Kind | Study these |
|------|-------------|
| Text + EN | `Text.vue` + `Text.cshtml` |
| Text + image | `TextImage.vue` + `TextImage.cshtml` |
| New scalar fields | `CountDown.vue` + `CountDown.cshtml` + `vueapps.ts` |
| List field | `Video.vue` + `Video.cshtml` |
| Album picker | `Music.vue` + `Music.cshtml` |
| ViewComponent only | `MemberList` / `PostList` |
| Form + TranslationsService | `Join.cshtml` |
| Empty editor shell | `MailBox.vue` (menu + partial; little/no editor UI) |

## Done when

- Widget appears in the correct add menu and shows the right header
- Editor persists fields after save/reload
- Public page renders the partial without errors
- EN language cookie shows `textEng` when applicable
- Interactive widgets mount once and work logged-in/out as intended
