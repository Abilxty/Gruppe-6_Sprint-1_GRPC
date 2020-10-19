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
        public override Task<ArtikelModell> GetArtikelInfo(ArtikelSuchenMitIdModell request, ServerCallContext context)
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
            else
            {
                output.Id = "3";
                output.Name = "Schrank";
                output.Anzahl = 42;
                output.MinBestand = 12;
            }

            return Task.FromResult(output);
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
