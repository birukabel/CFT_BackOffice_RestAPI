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
    public class MembershipController : Controller
    {
        private readonly IMembership _db;

        public MembershipController(IMembership db)
        {
            _db = db;
        }
        [Route("member")]
        [HttpGet]
        public JsonResult GetAllMember()
        {
            return new JsonResult(_db.GetMemberInformation());
        }
        [Route("client")]
        [HttpGet]
        public JsonResult GetClientByMember(Guid memberId)
        {
            return new JsonResult(_db.GetClentByMemberId(memberId));
        }
        [Route("businessLiceness")]
        [HttpGet]
        public JsonResult GetBusinessLiceness(Guid clientmemberId)
        {
            return new JsonResult(_db.GetBusinessLiceness(clientmemberId));
        }
        [Route("agreement")]
        [HttpGet]
        public JsonResult GetAggrement(Guid memberId,Guid clientId)
        {
            return new JsonResult(_db.GetAggremenet(memberId, clientId));
        }
        [Route("option")]
        [HttpGet]
        public JsonResult GetOption()
        {
            return new JsonResult(_db.GetOption());
        }
        [Route("GetMarginLookUp")]
        [HttpGet]
        public JsonResult GetMarginLookUp()
        {
            return new JsonResult(_db.GetMarginLookUp());
        }
    }
}
