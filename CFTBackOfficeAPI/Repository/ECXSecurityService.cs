using CFTBackOfficeAPI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using ECXSecurity;

namespace CFTBackOfficeAPI.Repository
{
    public class ECXSecurityService : IECXSecurityService
    {
        public readonly EndpointAddress endpointAddress;
        public readonly BasicHttpBinding basicHttpBinding;
        public readonly IConfiguration _configuration;
        public ECXSecurityService(IConfiguration configuration)
        {
            _configuration = configuration;

            endpointAddress = new EndpointAddress(_configuration["ECXSecurityService"]);

            basicHttpBinding = new BasicHttpBinding(endpointAddress.Uri.Scheme.ToLower() == "http" ?
                            BasicHttpSecurityMode.None : BasicHttpSecurityMode.Transport);
            basicHttpBinding.OpenTimeout = TimeSpan.MaxValue;
            basicHttpBinding.CloseTimeout = TimeSpan.MaxValue;
            basicHttpBinding.ReceiveTimeout = TimeSpan.MaxValue;
            basicHttpBinding.SendTimeout = TimeSpan.MaxValue;
        }
        public ECXSecurityAccessSoapClient GetService()
        {
            return new ECXSecurityAccessSoapClient(basicHttpBinding, endpointAddress);
        }
    }
}
