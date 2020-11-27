using ContentProvider.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentProvider.Services
{
    public interface IRoutesService
    {
        IEnumerable<MicroService> GetRandomServiceRoutes();

    }
}
