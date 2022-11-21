using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Models
{
    public class ContractDetail
    {
		public Guid ID { get; set; }
		public string ContractNumber { get; set; }
		public DateTime ContractDate { get; set; }
		public DateTime MaturityDate { get; set; }
		public Guid CFTBuyId { get; set; }
		public Guid CFTSellId { get; set; }
		public string OptionName { get; set; }
		public string Commodity { get; set; }
		public string CommodityClass { get; set; }
		public string Symbol { get; set; }
		public string Warehouse { get; set; }
		public string TraderWarehouse { get; set; }
		public int ProductionYear { get; set; }
		public decimal QuantityInLot { get; set; }
		public decimal QuantityNetWeight { get; set; }
		public decimal RemainingQuantity { get; set; }
		public string Price { get; set; }
		public string Attachement { get; set; }
		public string BuyerMargin { get; set; }
		public decimal BuyerMarginAmount { get; set; }
		public string SellerMargin { get; set; }
		public decimal SellerMarginAmount { get; set; }

	}
}
