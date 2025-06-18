# Instruktioner för installation

Utvecklat med VS 2022 (Visual studio code fungerar bra också).

Utveckling av frontend görs bäst med Visual studio code samt pluginet Vue - Official som är bäst för att hantera senaste versionen av Vue.

Rekommenderas inte att installera med äldre verktyg.

När du sätter upp en lokal utvecklingsmiljö så behöver du sätta upp en lokal databas(MySql5 eller Maria Db) samt lägga till en initial användare med /account/initnintendo . Då skapas bla användaren nintendo med lösen 123456.

# Databas orm:

[Entityframeworkcore](https://learn.microsoft.com/en-us/ef/core/) med [Mysql](https://www.mysql.com/) 5 alternativt Maria Db(Maria db är kompatibelt med MySQL 5 och är vad vi använder i produktion just nu). <br />

För lokal utveckling kan man skapa en lokal mysqldb. För att sätta upp den så kopierar man
appsettings.json.example och tar bort .example samt ersätter connectionsträngen med den som ska användas. Migrationer appliceras automatiskt vid uppstart av applikationen. Alternativt kan man köra Update-Database i package manager console.

Migrationer hanteras med Add-Migration och Update-Database i package manager console.

# Användare:

Identity:
https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity

# Bootstrap

Sidan använder Bootstrap 3. Läs mer om dess standardanvändning [här](http://getbootstrap.com/css/) och dess komponenter [här](http://getbootstrap.com/components/).
I nuläget har alla javascriptfiler från bootstrap plockats bort. Vi använder dock en hel del bootstrapstyling fortsatt, framför allt för layout.

# Vite, SCSS och TS:

JS, TS och scss-filer byggs med vite. Vill du lägga till en typescriptfil lägg till i main.ts under /Scripts. Vill du lägga till en scssfil lägg till i /Styles/akstyle.scss. I utvecklingsmiljön så byggs statiska filer om automatiskt.

## Installation

För att få frontendresurser att bygga krävs NPM (Node package manager). Det installeras genom att installera Node på din dator. Installera det samt ladda ner paketen som behövs med kommandot npm install (isntall fungerar också). Vite körs av .net och bör ej köras manuellt.

## Scss

[Scss](http://sass-lang.com/guide) är som css men med mer funktionalitet. Vet man inte vad man ska göra så skriv vanlig css och använd filändelsen scss så funkar det alldeles utmärkt.

## Vue.js

[Vue 3](https://vuejs.org/) används för vissa dynamiska komponenter som inloggning, men ännu mer på inloggade sidor för att hantera dynamiska element och liknar mycket react i funktionalitet. Vi använder Vues composition api samt typescript.
