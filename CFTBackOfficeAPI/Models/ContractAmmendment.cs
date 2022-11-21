using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Models
{
    public class ContractAmmendment
    {
        public Guid Id { get; set; }
        public Guid CommodityId { get; set; }
        public Guid CommodityClassId { get; set; }
        public string CommodityGrades { get; set; }
        public decimal QuantityInLot { get; set; }
        public decimal QuantityNetWeight { get; set; }
        public string Price { get; set; }
    }
}
