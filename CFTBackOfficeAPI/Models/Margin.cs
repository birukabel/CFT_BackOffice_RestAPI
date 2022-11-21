using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Models
{
    public class Margin
    {
        public int Id { get; set; }
        public Guid CFTId { get; set; }
        public Guid CFTTraderId { get; set; }
        public int MarginId { get; set; }
        public decimal Amount { get; set; }
        public decimal QuantityInLot { get; set; }
        public Guid AccountNumber { get; set; }
        public string Side { get; set; }
    }
}
