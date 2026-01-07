MindMatch – Mantelzorger & Gebruiker App

MindMatch is een Unity-applicatie die mantelzorgers en gebruikers met elkaar verbindt.
Mantelzorgers kunnen taken en handleidingen (met foto’s) aanmaken, terwijl gebruikers deze overzichtelijk kunnen bekijken.
De app maakt gebruik van Firebase voor authenticatie en cloudopslag.

Functionaliteiten
Mantelzorger

Registreren & inloggen

Rol kiezen als Mantelzorger

Taken toevoegen, verwijderen en beheren

Taken instellen met:
Datum
Dagelijkse herhaling

Handleidingen aanmaken
Tekst
Foto’s (opgeslagen in Firebase Storage)
Deel een unieke code om een gebruiker te koppelen

Gebruiker

Rol kiezen als Gebruiker
Koppelen aan een mantelzorger via code

Bekijken van:
Taken (read-only)
Handleidingen en foto’s


Technologieën

Unity (C#)

Firebase
Firebase Authentication (Email/Password)
Cloud Firestore (taken, handleidingen)
Firebase Storage (foto’s)
Native Gallery
Native Date Picker
TextMeshPro
Unity UI


Vereisten

Unity (2021 LTS of nieuwer aanbevolen)

Firebase Unity SDK

Internetverbinding voor eerste synchronisatie

Firebase project met:

Email/Password Authentication ingeschakeld

Firestore database

Firebase Storage

Testen

De app kan zo ingesteld worden dat bij elke start:

De rol opnieuw gekozen moet worden

Dit is handig voor development en demo’s



Auteur

Bram Eylenbosch
Eindwerk Multimedia / Unity Project
