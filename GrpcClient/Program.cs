using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var input = new HelloRequest { Name = "Tim" };
            //var channel = GrpcChannel.ForAddress("https://localhost:5001");
            /*var client = new Greeter.GreeterClient(channel);

            var reply = await client.SayHelloAsync(input);

            Console.WriteLine(reply.Message);
            */


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

                Console.ReadLine();
        }
    }
}
