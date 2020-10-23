using Grpc.Core;
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
        public override Task<ArtikelModell> GetArtikelInfo(
            ArtikelSuchenMitIdModell request, ServerCallContext context)
        {
            ArtikelModell output = new ArtikelModell();
            if (request.Id == "1")
            {
                output.Id = "1";
                output.Name= "Stuhl";
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
            else if(request.Id == "3")
            {
                output.Id = "3";
                output.Name = "Schrank";
                output.Anzahl = 42;
                output.MinBestand = 12;
            }
           // else
            //{
             //   throw StatusCode.NotFound;
           // }

            return Task.FromResult(output);
        }

        // Überschreiben der GetAlleArtikel Methode
        // Das IEnumerable-Objekt wird iteriert und der jeweilige Artikel wird asynchron gestreamed
        public override async Task GetAlleArtikel(AlleArtikelAnfrage request, IServerStreamWriter<ArtikelModell> responseStream, ServerCallContext context)
        {
            foreach (var current in GetArtikels())
            {
                await responseStream.WriteAsync(current);
            }
        }
        // Überschreiben der GetAlleArtikelKollektion Methode - Gibt alle Artikel einer bestimmmten Kollektion zurück
        // 
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

        public IEnumerable<ArtikelModell> GetArtikels(string kol=null)
        {
            List<ArtikelModell> dummyListe = new List<ArtikelModell>();
            dummyListe.Add(new ArtikelModell { Anzahl = 10, Id = "1", IstAusverkauft=false, MinBestand=5, Name="Stuhl", Kollektion="a"});
            dummyListe.Add(new ArtikelModell { Anzahl = 25, Id = "2", IstAusverkauft=false, MinBestand=3, Name="Tisch", Kollektion="a"});
            dummyListe.Add(new ArtikelModell { Anzahl = 42, Id = "3", IstAusverkauft=false, MinBestand=12, Name="Schrank", Kollektion="a"});
            dummyListe.Add(new ArtikelModell { Anzahl = 0, Id = "4", IstAusverkauft=true, MinBestand=7, Name="Lampe", Kollektion="b"});
            dummyListe.Add(new ArtikelModell { Anzahl = 0, Id = "5", IstAusverkauft=true, MinBestand=12, Name="Schreibtisch", Kollektion="b"});
            dummyListe.Add(new ArtikelModell { Anzahl = 0, Id = "6", IstAusverkauft=true, MinBestand=20, Name="Nachttisch", Kollektion="b"});

            if (kol != null)
            {
                foreach (var artikel in dummyListe)
                {
                    if(artikel.Kollektion == kol)
                    {
                        yield return artikel;
                    }
                }
            }

            foreach (var artikel in dummyListe)
            {
                yield return artikel;
            }

        }

        
        /*public override async Task GetNewCustomers(NewCustomerRequest request, IServerStreamWriter<CustomerModel> responseStream, ServerCallContext context)
        {
            List<CustomerModel> customers = new List<CustomerModel>
            {
                new CustomerModel
                {
                    FirstName = "Furkan",
                    LastName = "Asani",
                    EmailAddress = "asfu1011@hs-karlsruhe.de",
                    Age = 21,
                    IsAlive = true
                },
                new CustomerModel
                {
                    FirstName = "Erkan",
                    LastName = "Asani",
                    EmailAddress = "aser1011@hs-karlsruhe.de",
                    Age = 19,
                    IsAlive = true
                },

            };
            foreach (var cust in customers)
            {
                await Task.Delay(1000);
                responseStream.WriteAsync(cust);
            }
        }*/
    }
    
}
