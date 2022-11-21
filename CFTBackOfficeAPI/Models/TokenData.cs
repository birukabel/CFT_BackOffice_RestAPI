using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Models
{
    public class TokenData
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string ExpireAt { get; set; }

    }
}
