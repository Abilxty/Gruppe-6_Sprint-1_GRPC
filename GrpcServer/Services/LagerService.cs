﻿using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Services
{
    public class LagerService : Lager.LagerBase
    {
        private readonly ILogger<LagerService> _logger;

        public LagerService(ILogger<LagerService> logger)
        {
            _logger = logger;
        }

        /// Anstelle eines Aufrufs einer Datenbank simulieren wir dies durch diese Liste.
        List<ArtikelModell> dummyListe = new List<ArtikelModell>()
        {
            new ArtikelModell { Anzahl = 10, Id = "1", IstAusverkauft=false, MinBestand=5, Name="Stuhl", Kollektion="a", Preis=5},          
            new ArtikelModell { Anzahl = 0, Id = "5", IstAusverkauft = true, MinBestand = 12, Name = "Schreibtisch", Kollektion = "b", Preis=6 },
            new ArtikelModell { Anzahl = 25, Id = "2", IstAusverkauft=false, MinBestand=3, Name="Tisch", Kollektion="a", Preis =8},
            new ArtikelModell { Anzahl = 0, Id = "6", IstAusverkauft = true, MinBestand = 20, Name = "Nachttisch", Kollektion = "b", Preis=10 },
            new ArtikelModell { Anzahl = 42, Id = "3", IstAusverkauft = false, MinBestand = 12, Name = "Schrank", Kollektion = "a", Preis=42 },
            new ArtikelModell { Anzahl = 0, Id = "4", IstAusverkauft = true, MinBestand = 7, Name = "Lampe", Kollektion = "b", Preis=2 },
    };
       

        /// <summary>
        /// Überschreiben der GetArtikelInfo-Methode. Diesse Methode kriegt einen ArtikelSuchenMitIdModell-Request und gibt gegebenenfalls den gesuchten Artikel zurück.
        /// </summary>
        /// <param name="request"></param> Der Request, welcher vom Client abgesetzt wurde.
        /// <param name="context"></param> Der Context des ServerCalls
        /// <returns></returns>
        public override async Task<ArtikelModell> GetArtikelInfo(
                    ArtikelSuchenMitIdModell request, ServerCallContext context)
                {
                    ArtikelModell output = new ArtikelModell();


                    try
                    {
                        if (request.Id == "1")
                        {
                            output.Id = "1";
                            output.Name = "Stuhl";
                            output.Anzahl = 10;
                            output.MinBestand = 5;
                        }
                        else if (request.Id == "2")
                        {
                            output.Id = "2";
                            output.Name = "Tisch";
                            output.Anzahl = 25;
                            output.MinBestand = 3;

                        }
                        else if (request.Id == "3")
                        {
                            output.Id = "3";
                            output.Name = "Schrank";
                            output.Anzahl = 42;
                            output.MinBestand = 12;
                        }
                        else
                        {
                            throw new RpcException(new Status(StatusCode.InvalidArgument, "Ouch!"));
                        }
                    }
                    catch(RpcException e)
                    {
                        Console.WriteLine($"Ein Fehler ist aufgetreten: {e.Message} \nStatusCode: {e.StatusCode}");
                        output.StatusCode = "404";
                    }

                    return await Task.FromResult(output);
                }
        /// <summary>
        /// Eine Methode, welche einen Bestellung1ArtikelRequest übergeben bekommt und daraufhin eine Bestellbestätigung(vom Datentyp TriggerBestellungResult) zurückgibt.
        /// </summary>
        /// <param name="request"></param> Der übergebene Request.
        /// <param name="context"></param> Der Context des ServerCalls
        /// <returns></returns>
        public override async Task<TriggerBestellungResult> TriggerBestellung(Bestellung1Artikel request, ServerCallContext context)
        {

            TriggerBestellungResult output = new TriggerBestellungResult();
            // "Auslesen" aus der Datenbank
            var gewünschterArtikel = (await GetArtikelInfo(new ArtikelSuchenMitIdModell { Id = request.Id }, context));
            int verfügbareAnzahl = gewünschterArtikel.Anzahl;

            try
            {   // Checken, ob der Bestand größer ist als die angefragte Menge. Falls dies nicht der Fall ist, wird eine Exception geworfen.
                if (verfügbareAnzahl < request.Anzahl)
                {
                    output.StatusCode = "501";
                    return output;
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, "Der Bestand ist zu niedrig!"));
                }
            }
            catch(RpcException e)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten: {e.Message} \nStatusCode: {e.StatusCode}");
                
            }

            int gesamtPreis;
            int artPreis=100;
            // "Datenbank-Query" welche uns den gesuchten Preis zurückgibt, damit wir den Gesamtpreis berechnen können.
            foreach(ArtikelModell art in dummyListe)
            {
                if(gewünschterArtikel.Id == art.Id)
                {
                    artPreis = art.Preis;
                    art.Anzahl=art.Anzahl-1;
                    gesamtPreis = art.Preis * request.Anzahl;
                    if (art.Anzahl < art.MinBestand)
                    {
                        Console.WriteLine("Der aktuelle Bestand ist kleiner als der Mindestbestand! Es sollte unbedingt nachbestellt werden!");
                    }
                }
            }
            output.Preis = artPreis;
            output.StatusCode = "201";
            return output;
        }

        /// <summary>
        /// Überschreiben der GetAlleArtikel Methode. Das IEnumerable-Objekt, welches von der Methode GetArtikels() zurückgegeben wird, wird iteriert und der jeweilige Artikel wird asynchron gestreamed.
        /// </summary>
        /// <param name="request"></param> Der einkommende Request des Clients
        /// <param name="responseStream"></param> Der ausgehende Response vom Server
        /// <param name="context"></param> Der Context des ServerCalls
        /// <returns></returns>
        public override async Task GetAlleArtikel(AlleArtikelAnfrage request, IServerStreamWriter<ArtikelModell> responseStream, ServerCallContext context)
        {
            foreach (var current in GetArtikels())
            {
                await responseStream.WriteAsync(current);
            }
        }
        // 
        //
        /// <summary>
        /// Überschreiben der GetAlleArtikelKollektion Methode - Gibt alle Artikel einer bestimmmten Kollektion zurück 
        /// </summary>
        /// <param name="requestStream"></param> Der RequestStream --> die Daten, die der Client an den Server schickt
        /// <param name="responseStream"></param> Der ResponseStream --> die Daten, welcher der Server zurück an den Client sendet
        /// <param name="context"></param> Der Context des ServerCalls
        /// <returns></returns>
        public override async Task GetAlleArtikelKollektion(IAsyncStreamReader<Kollektion> requestStream, IServerStreamWriter<ArtikelModell> responseStream, ServerCallContext context)
        {
            int counter = 0;
            while (await requestStream.MoveNext())
            {
                var curKol = requestStream.Current;
                List<Kollektion> allKol = new List<Kollektion>();
                allKol.Add(curKol);


                foreach (var current in GetArtikels(allKol[counter].Kol))
                {
                    await responseStream.WriteAsync(current);
                }
            }
        }


        /// <summary>
        /// Methode, welche ein IEnumerable zurückgibt vom Typ ArtikelModell. Hiermit können entweder alle Artikel zurückgeben werden oder nur Artikel einer bestimmten Kollektion.
        /// </summary>
        /// <param name="kollektionsString"></param> kann gesetzt werden, wenn nur nach bestimmten Kollektionen gesucht werden soll. Falls alle Artikel angezeigt werden sollen wird dieser Parameter freigelassen.
        /// <returns></returns>
        public IEnumerable<ArtikelModell> GetArtikels(string kollektionsString=null)
        {

            // Wenn der kollektionString gesetzt wurde, dann iterieren wir über jeden Artikel aber geben nur diejenigen zurück, die unserer gewünschten Kollektion entsprechen.
            if (kollektionsString != null)
            {
                foreach (var artikel in dummyListe)
                {
                    if(artikel.Kollektion == kollektionsString)
                    {
                        yield return artikel;
                    }
                }
            }
            // Falls der kollektionsString nicht gesetzt wurde iterieren wir über die Liste und geben jeden Artikel zurück.
            else
            {
                foreach (var artikel in dummyListe)
                {
                    yield return artikel;
                }
            }
            

        }

    }
    
}
