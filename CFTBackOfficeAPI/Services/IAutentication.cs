using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Services
{
    public interface IAutentication
    {
        public Task<string> AutenticateUser(string userName, string password);      
    }
}
