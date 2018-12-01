# Instruktioner för installation

Utvecklat med VS 2017 (Visual studio code fungerar bra också).

Rekomenderas inte att installera med äldre verktyg.

När du sätter upp en lokal utvecklingsmiljö så behöver du sätta upp en lokal mysql databas samt lägga till en initial användare med /account/initnintendo . Då skapas bla användaren nintendo med lösen 123456.

# Databas orm:
[Entityframeworkcore](https://docs.efproject.net/en/latest/) med [Mysql](https://www.mysql.com/). <br />

För lokal utveckling kan man skapa en lokal mysqldb. För att sätta upp den så kopierar man 
appsettings.json.example och tar bort .example samt ersätter connectionsträngen med den som ska användas. Efter det kör man Update-Database i package manager console.

Migrationer hanteras med Add-Migration och Update-Database i package manager console.

# Användare:
Identity: 
https://docs.asp.net/en/latest/security/authentication/identity.html

# Bootstrap
Som frontendframework används Bootstrap 3. Läs mer om dess standardanvändning [här](http://getbootstrap.com/css/) och dess komponenter [här](http://getbootstrap.com/components/).

# Webpack CSS och JS:

JS och scssfiler byggs med webpack. Vill du lägga till en javascriptfil lägg till i main.js under scripts. Vill du lägga till en scssfil lägg till i /Styles/akstyle.scss. I developmentmiljö så byggs statiska filer om automatiskt.

## Installation

För att få frontendresurser att bygga krävs NPM (Node package manager). Installera det samt ladda ner paketen som behövs med kommandot npm install (isntall fungerar också). Webpack körs av webbinstansen och bör ej köras manuellt.

## Scss

[Scss](http://sass-lang.com/guide) är som css men med mer funktionalitet. Vet man inte vad man ska göra så skriv vanlig css och använd filändelsen scss så funkar det alldeles utmärkt.

## Vue.js

[Vue](https://vuejs.org/) används för vissa inloggade sidor för att hantera dynamiska element och liknar mycket react i funktionalitet.
