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
            var artikelRequested = new ArtikelSuchenMitIdModell { Id = "2" };

            // Die übergebene ID wird der Methode GetArtikelInfoAsync übergeben, welche die Artikelinfos zurückgibt.
            var artikel = await lagerClient.GetArtikelInfoAsync(artikelRequested);

            // Formatierte Ausgabe der Artikelinfos
            Console.WriteLine($" ID : {artikel.Id} \n Name : {artikel.Name} \n Anzahl : {artikel.Anzahl} \n Ausverkauft : {artikel.IstAusverkauft}");

            // Es wird eine Anfrage nach allen Artikeln übermittelt 
            using var alleArtikel = lagerClient.GetAlleArtikel(new AlleArtikelAnfrage());
            
            // Der ResponseStream wird mit Hilfe einer Schleife durchgegangen und der jeweilige Artikel wird auf der Konsole freigegeben
            await foreach (var art in alleArtikel.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($" ID : {art.Id} \n Name : {art.Name} \n Anzahl : {art.Anzahl} \n Ausverkauft : {art.IstAusverkauft}");
            }

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
                for( int i=0; i<2; ++i)
                {
                    Console.WriteLine("Bitte geben sie ihre gewünschte Kollektion ein!");
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
