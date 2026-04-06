# SystemIntegration Lektion 8

## Opgave 1

Kør topics eksempelt. 

*RecieveLogTopics:*
> dotnet run pattern

*EmitLogTopics:*
> dotnet run routingKey besked

Afprøv forskellige routing keys og se hvordan det påvirker hvilke beskeder der kommer frem i RecieveLogTopics.

## Opgave 2

*Bonusopgave fra afleveringen*

Implementer forskellige "skærmtyper" ved hjælp af RabbitMQ Topics. Forskellige skærmtyper kunne være indenrigs og udenrigs terminaler. 
Lav et endpoint eller modificer eksistrerende endpoints, så man kan sende information ud til en bestemt type terminaler.

## Opgave 3 

Vores taxi selvskab vil give kunder mulighed for at bestille en mere specifik taxi. I første omgang vil der to forskellige parameter 
der kan rekvireres. Størrelse: Det skal være muligt at bestille en taxi 6 personer. Drivmiddel: Det skal være muligt 
at bestille en taxi der kører på el. 

Implementer et system der kan håndtere disse bestillinger. Det skal være muligt at bestille en taxi med en eller begge parametre.
Hvis kunden ikke specificerer nogen parametre, skal denne ordre sendes til alle taxier. Hvis kunden specificerer størrelse, skal denne ordre sendes til alle taxier der kan håndtere 6 personer. 
Hvis kunden specificerer drivmiddel, skal denne ordre sendes til alle taxier der kører på el. Hvis kunden specificerer begge parametre, skal denne ordre sendes til alle taxier der kan håndtere 6 personer og kører på el.