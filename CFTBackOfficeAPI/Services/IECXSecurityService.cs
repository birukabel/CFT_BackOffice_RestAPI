using ECXSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Services
{
   public interface IECXSecurityService
    {
        public ECXSecurityAccessSoapClient GetService();
    }
}
