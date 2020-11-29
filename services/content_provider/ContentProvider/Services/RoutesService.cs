using ContentProvider.DTO;
using ContentProvider.Utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentProvider.Services
{
    public class RoutesService: IRoutesService
    {

        private readonly IConfiguration configuration;

        public RoutesService(IConfiguration configuration) 
        {
            this.configuration = configuration;
        }

        public IEnumerable<MicroService> GetRandomServiceRoutes()
        {

            var result = new List<MicroService>();
            var microServices = configuration.GetSection("MicroServices").GetChildren();
            foreach (var microService in microServices)
            {
                var microServiceName = microService.GetSection("ServiceName:0").Value;
                var microServiceRoutes = microService.GetSection("ServiceEndpoints").Get<List<string>>();

                MicroService selectedMicroserviceRoutes = new MicroService { 
                    Name = microServiceName,
                    Endpoints = new List<string>()
                };

                if (microServiceRoutes.Count > 3)
                {
                    selectedMicroserviceRoutes.Endpoints.AddRange(microServiceRoutes.GetRandom(3));
                }
                else {
                    selectedMicroserviceRoutes.Endpoints.AddRange(microServiceRoutes);
                }

                result.Add(selectedMicroserviceRoutes);
             
            }

            return result;

        }
    }
}
