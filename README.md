# Instruktioner för installation

Utvecklat med VS 2017

Rekomenderas inte att installera med äldre verktyg.

Om du rensar databasen som kommer med i versionshanteringen så behöver du skapa initiala användare med /account/initnintendo Då skapas bla användaren nintendo med lösen 123456.

# Databas orm:
[Entityframeworkcore](https://docs.efproject.net/en/latest/) med [SQLite](http://ef.readthedocs.io/en/latest/providers/sqlite/). <br />
EFCore gillar att man använder .Include() då EF core ej har [lazyloading](https://docs.efproject.net/en/latest/querying/related-data.html) av referenser.

SQLite saknar också förmågan att ta bort kolumner ur Databasen vilket gör att många migrationer misslyckas. Lös genom att ta bort db och migrationer och gör sedan en Add-Migration och Update-Database. Eller så kan man byta till en annan db. <br />
![xkcdbild](http://imgs.xkcd.com/comics/git.png)

Migrationer hanteras med Add-Migration och Update-Database i package manager console.

Använd gärna annan databas via EF men mysql har än så länge inget stöd i CORE.

#Användare:
identity: 
https://docs.asp.net/en/latest/security/authentication/identity.html

#Bootstrap
Som frontendframework används Bootstrap 3. Läs mer om dess standardanvändning [här](http://getbootstrap.com/css/) och dess komponenter [här](http://getbootstrap.com/components/).

#CSS och JS:

JS och scssfiler (css) bundlas med gulp. Vill du lägga till en javascriptfil lägg till i gulpfilens lista. Vill du lägga till en scssfil lägg till i /Styles/akstyle.scss.

##Scss

[Scss](http://sass-lang.com/guide) är som css men med mer funktionalitet. Vet man inte vad man ska göra så skriv vanlig css och använd filändelsen scss så funkar det alldeles utmärkt.
