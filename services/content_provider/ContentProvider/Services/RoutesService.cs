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
            var microServices = configuration.GetSection("MicroServices").Get<List<MicroService>>();
            foreach (var microService in microServices)
            {
                MicroService selectedMicroserviceRoutes = new MicroService { 
                    Name = microService.Name,
                    Routes = new List<string>()
                };

                if (microService.Routes.Count > 3)
                {
                    selectedMicroserviceRoutes.Routes.AddRange(microService.Routes.GetRandom(3));
                }
                else {
                    selectedMicroserviceRoutes.Routes.AddRange(microService.Routes);
                }

                result.Add(selectedMicroserviceRoutes);
             
            }

            return result;

        }
    }
}
