using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Models
{
    public class ContractDTO
    {
        public Contract Contract{get;set;}
        public ContractTrader Seller{get;set;}
        public ContractTrader Buyer { get; set; }

    }
}
