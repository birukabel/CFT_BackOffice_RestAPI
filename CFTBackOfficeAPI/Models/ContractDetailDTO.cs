using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Models
{
    public class ContractDetailDTO
    {
        public ContractDetail detail { get; set; }
        public List<ContractTrader> trader { get; set; }
    }
}
