using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NOAAWeatherServiceContract
{
    [ServiceContract]
    public interface INOAAWeatherServiceManager : IDisposable
    {
        [OperationContract]
        Task<string> GetCurrent(string zipCode);
        [OperationContract]
        Task<string> GetForecast(string zipCode);
    }
}
