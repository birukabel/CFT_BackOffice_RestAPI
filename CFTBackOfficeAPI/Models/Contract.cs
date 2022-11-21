using Microsoft.AspNetCore.Http;
using System;

namespace CFTBackOfficeAPI.Models
{
   public class Contract
    {
        public Guid ID { get; set; }
        public String ContractNumber { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public Guid CFTBuyId { get; set; }
        public Guid CFTSellId { get; set; }
        public int OptionId { get; set; }
        public int StatusId { get; set; }
        public Guid CommodityId { get; set; }
        public Guid CommodityClassId { get; set; }
        public string Symbol { get; set; }
        public Guid ECXWarehouseId { get; set; }
        public string TraderWarehouse { get; set; }
        public int ProductionYear { get; set; }
        public decimal QuantityInLot { get; set; }
        public decimal QuantityNetWeight { get; set; }
        public decimal RemainingQuantity { get; set; }
        public string Price { get; set; }
        public string Remark { get; set; }
        public IFormFile Attachement { get; set; }
        public int BuyerMargin { get; set; }
        public int SellerMargin { get; set; }
        public Guid MakerId { get; set; }
        public DateTime MakerDate { get; set; }
        public Guid CheckerId { get; set; }
        public DateTime CheckerDate { get; set; }

    }
}
