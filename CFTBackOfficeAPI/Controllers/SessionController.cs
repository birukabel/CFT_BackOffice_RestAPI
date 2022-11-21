using CFTBackOfficeAPI.Models;
using CFTBackOfficeAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace CFTBackOfficeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "ContractViewer,ContractChecker,ContractMaker,SellMarginMaker,SellMarginChecker,BuyMarginChecker,BuyMarginMaker,SessionTemplateChk,SessionStatus")]
    public class SessionController : ControllerBase
    {
        private readonly IContractSession _db;

        public SessionController(IContractSession db)
        {
            _db = db;
        }
        [HttpGet]
        [Authorize (Roles= "SessionStatus,SessionTemplateChk") ]
        public ActionResult<List<ContractSession>> GetSession()
        {
            List<ContractSession> data  = _db.GetAllSession();
            
            return data;
            
        }
        [HttpPost]
        [Authorize(Roles = "SessionTemplateChk")]
        public IActionResult SaveSession(Guid createdBy)
        {
            var result =  _db.Save(createdBy);
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while saving session");
            }
            return Ok("Added Successfully");
        }
        [HttpPut]
        [Authorize(Roles = "SessionStatus")]
        public IActionResult UpdateSession([FromBody] ContractSession session)
        {
           
            var result = _db.Update(session.ID, session.Status,new Guid(session.UpdatedBy.ToString()));
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while updating session");
            }
            return Ok("Updated Successfully");
        }
        [HttpGet("audit")]
        [Authorize(Roles = "SessionStatus")]
        public IActionResult GetSessionAudiTrial()
        {
            DataTable data = _db.GetSessionAuditTrial();
            return new OkObjectResult(data);


        }
    }
}
