Utvecklat med VS 2015 Update 3 + .NET CORE 

Rekomenderas inte att installera med äldre verktyg.

DB:
Entityframeworkcore med SQLite
EFCore gillar att man använder .Include() förreferenser till andra tabeller.

Använd gärna annan databas via EF men mysql har än så länge inget stöd i CORE.

Fil för db sätts i AkContext.OnConfiguring vore snyggare att kontrollera via config.

Användare:

identity: 
https://docs.asp.net/en/latest/security/authentication/identity.html

Skapar initiala användare samt roller i accountcontroller/initnintendo. Bör fixas snyggare via configfiler senare.


CSS och JS:

JS bundlas av bundler minifier och less-filer hanteras av gulp då jag inte hittade lesshantering för bundlerminifyer. Bord förmodligen fixa enhetlig lösning.