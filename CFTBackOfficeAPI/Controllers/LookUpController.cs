using CFTBackOfficeAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ContractViewer,ContractChecker,ContractMaker,SellMarginMaker,SellMarginChecker,BuyMarginChecker,BuyMarginMaker,SessionTemplateChk,SessionStatus")]
    public class LookUpController : Controller
    {
        private readonly ILookUp _db;

        public LookUpController(ILookUp db)
        {
            _db = db;
        }
        [Route("commodity")]
        [HttpGet]
        
        public JsonResult GetCommodity()
        {
            return new JsonResult(_db.GetCommodity());
        }
        [Route("commodityClass")]
        [HttpGet]
        
        public JsonResult GetCommodityClass(Guid commodityId)
        {
            return new JsonResult(_db.GetCommodityClassByCommodity(commodityId));
        }
        [Route("commodityGrade")]
        [HttpGet]
        
        public JsonResult GetCommodityGrade(Guid classId)
        {
            return new JsonResult(_db.GetCommodityGradeByClassId(classId));
        }
        [Route("warehouse")]
        [HttpGet]
        
        public JsonResult GetWarehouse()
        {
            return new JsonResult(_db.GetWarehouse());
        }
       
        [HttpGet("bankAccount")]
        public JsonResult GetBankAccountByOwner(Guid TraderId)
        {
            var tbl = _db.GetBankAccountByOwner(TraderId);
            return new JsonResult(_db.GetBankAccountByOwner(TraderId));
        }

    }
}
