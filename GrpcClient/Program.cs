using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // langlebige HTTP2 Verbindung aufbauen
            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            // Client wird angelegt und diesem wird der Channel übergeben
            var lagerClient = new Lager.LagerClient(channel);
            
            // Die ID-Übergabe wird simuliert
            var artikelRequested = new ArtikelSuchenMitIdModell { Id = "4" };

            // Die übergebene ID wird der Methode GetArtikelInfoAsync übergeben, welche die Artikelinfos zurückgibt.
            var artikel = await lagerClient.GetArtikelInfoAsync(artikelRequested);

            // Formatierte Ausgabe der Artikelinfos
           if(artikel.StatusCode != null)
            {
                Console.WriteLine($"Es ist ein Fehler aufgetreten StatusCode:{artikel.StatusCode}");
            }
            else
            {
                Console.WriteLine($" ID : {artikel.Id} \n Name : {artikel.Name} \n Anzahl : {artikel.Anzahl} \n Ausverkauft : {artikel.IstAusverkauft}");
            }

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            // Der Methode TriggerBestellung wird eine valide Anfrage übergeben, welche erfolgreich durchlaufen sollte. 
            var bestellungResponse = await lagerClient.TriggerBestellungAsync(new Bestellung1Artikel { Anzahl = 1, Id = "1" });
            if (bestellungResponse.StatusCode == "501")
            {
                Console.WriteLine("Der Artikel ist leider ausverkauft!");
            }
            else
            {
                Console.WriteLine($"Vielen Dank für deine Bestellung der Gesamtpreis beträgt {bestellungResponse.Preis} Euro");
            }

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            // Der Methode TriggerBestellung wird eine invalide Anfrage übergeben, welche zu einem Fehler führt. 
            var bestellungResponse2 = await lagerClient.TriggerBestellungAsync(new Bestellung1Artikel { Anzahl = 1, Id = "4" });
            if (bestellungResponse2.StatusCode == "501")
            {
                Console.WriteLine("Der Bestand für diesen Artikel ist leider nicht hoch genug!");
            }
            if (bestellungResponse2.StatusCode == "201")
            {
                Console.WriteLine($"Vielen Dank für deine Bestellung der Gesamtpreis beträgt {bestellungResponse.Preis} Euro");
            }

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            // Es wird eine Anfrage nach allen Artikeln übermittelt 
            using var alleArtikel = lagerClient.GetAlleArtikel(new AlleArtikelAnfrage());
            
            // Der ResponseStream wird mit Hilfe einer Schleife durchgegangen und der jeweilige Artikel wird auf der Konsole freigegeben
            await foreach (var art in alleArtikel.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($" ID : {art.Id} \n Name : {art.Name} \n Anzahl : {art.Anzahl} \n Ausverkauft : {art.IstAusverkauft}");
            }
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            // Der Responsestream beim Bidirektionalen Streaming wird hier durchiteriert und nacheinander ausgegeben.
            using (var call = lagerClient.GetAlleArtikelKollektion())
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var artikelKollektion = call.ResponseStream.Current;
                        Console.WriteLine($" ID : {artikelKollektion.Id} Name : {artikelKollektion.Name} Anzahl : {artikelKollektion.Anzahl} Ausverkauft : {artikelKollektion.IstAusverkauft} Kollektion : {artikelKollektion.Kollektion}");
                    }
                });

                // Simulation eines Requeststreams in Form von Eingabeaufforderungen
                Console.WriteLine("Bitte geben sie ihre gewünschte Kollektion ein! Sie können auch mehrere nacheinander eingeben.");
                for ( int i=0; i<2; ++i)
                {
                    String kolString = Console.ReadLine();
                    await call.RequestStream.WriteAsync(new Kollektion { Kol = kolString});
                }
                await call.RequestStream.CompleteAsync();
                await responseReaderTask;
            }

           
            Console.ReadLine();
        }
    }
}
