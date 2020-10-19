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

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var lagerClient = new Lager.LagerClient(channel);
            
            var artikelRequested = new ArtikelSuchenMitIdModell { Id = "1" };

            var artikel = await lagerClient.GetArtikelInfoAsync(artikelRequested);

            Console.WriteLine($" ID : {artikel.Id} \n Name : {artikel.Name} \n Anzahl : {artikel.Anzahl} \n Ausverkauft : {artikel.IstAusverkauft}");


           /* Console.WriteLine($"{artikel.id} {artikel.LastName}");
            Console.WriteLine("Net Customer List");

            using (var call = customerClient.GetNewCustomers(new NewCustomerRequest()))
            {
                while( await call.ResponseStream.MoveNext())
                {
                    var currentCustomer = call.ResponseStream.Current;
                    Console.WriteLine($"{currentCustomer.FirstName} {currentCustomer.LastName} : {currentCustomer.EmailAddress}");
                }
            }*/

                Console.ReadLine();
        }
    }
}
