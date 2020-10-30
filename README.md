# GrpcLager

#WICHTIG# 

Folgende Pakete sollten über den NuGet-Paketmanager installiert werden:

-Google.Protobuf

-Grpc.Net.Client

-Grpc.Tools

#WICHTIG


Dies ist ein Demo-Projekt im Rahmen der Vorlesung Automatisierung von Geschäftsprozessen.
Wir haben uns dafür entschieden einen Lagerservice zu implementieren, welcher einem folgende Funktionalität liefert:
---------------------------------------------------------------------------------------------------------------------

-Falls man die ID eines Artikels hat, kann man sich weitere Informationen zu besagtem Artikel holen.

rpc GetArtikelInfo

---------------------------------------------------------------------------------------------------------------------

-Falls man sich einen Überblick über das Lager verschaffen möchte kann sich alle Artikel anzeigen lassen.

rpc GetAlleArtikel

---------------------------------------------------------------------------------------------------------------------

-Es können alle Artikeln einer bestimmten Kollektion ausgegeben werden.

rpc GetAlleArtikelKollektion

---------------------------------------------------------------------------------------------------------------------

-Der Client kann, wenn er die ID des gewünschten Artikels hat, eine Bestellung tätigen.

rpc TriggerBestellung

---------------------------------------------------------------------------------------------------------------------

