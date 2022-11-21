using CFTBackOfficeAPI.Models;
using CFTBackOfficeAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace CFTBackOfficeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : Controller
    {
        private readonly IContract _db;
        private readonly IContractValidator _cv;
        private IConfiguration _cs;
        public ContractController(IContract db, IContractValidator cv, IConfiguration cs)
        {
            _db = db;
            _cs = cs;
            _cv = cv;
        }
        [HttpPost]
        [Authorize(Roles = "ContractMaker")]
        public JsonResult SaveContract([FromForm] ContractDTO _contract)
        {
            return new JsonResult(_db.Save(_contract, new Guid(HttpContext.User.FindFirst("id").Value)));
        }
        [HttpPut("update")]
        [Authorize(Roles = "ContractMaker")]
        public JsonResult UpdateContract([FromForm] ContractDTO _contract)
        {
            return new JsonResult(_db.Update(_contract, new Guid(HttpContext.User.FindFirst("id").Value)));
        }
        [Route("all")]
        [HttpGet]
        [Authorize(Roles = "ContractViewer")]
        public JsonResult GetAllContract()
        {
            return new JsonResult(_db.GetAllContract());
        }
        [Route("detail")]
        [HttpGet]
        [Authorize(Roles = "ContractViewer")]
        public JsonResult GetContractDetail(Guid id)
        {
            return new JsonResult(_db.GetDetailContract(id));
        }
        [Route("ammendment")]
        [HttpGet]
        [Authorize(Roles = "ContractMaker")]
        public JsonResult GetContractForAmmendment(Guid id)
        {
            return new JsonResult(_db.GetContractForAmmendment(id));
        }
        [Route("ammend")]
        [HttpPut]
        [Authorize(Roles = "ContractMaker")]
        public JsonResult AmmendmentContract([FromForm] ContractAmmendment _ammendment)
        {
            return new JsonResult(_db.SaveAmmendment(_ammendment, new Guid(HttpContext.User.FindFirst("id").Value)));
        }
        [HttpGet("downloadfile")]
        [Authorize(Roles = "ContractViewer")]
        public IActionResult GetContractFile(Guid contractId)
        {
            List<ContractDetail> detail = _db.GetContractById(contractId);
            if (detail.Count > 0 && detail[0].Attachement != "")
            {
                string physicalPath = _cs["AttachementDirectory"] + "\\" + detail[0].Attachement;
                if (System.IO.File.Exists(physicalPath))
                {
                    byte[] pdfBytes = System.IO.File.ReadAllBytes(physicalPath);
                    MemoryStream ms = new(pdfBytes);
                    return Ok(new FileStreamResult(ms, "application/pdf"));
                }
                return BadRequest("Unable to get file");
            }
            return NoContent();
            /* string localFilePath;
             List<ContractDetail> detail = _db.GetContractById(contractId);
             if (detail.Count > 0 && detail[0].Attachement != "")
             {
                 localFilePath = _cs["AttachementDirectory"] + "//" + detail[0].Attachement;


                 HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                 response.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
                 response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                 response.Content.Headers.ContentDisposition.FileName = detail[0].Attachement;
                 response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                 return response; 
             }
             return new HttpResponseMessage(HttpStatusCode.NoContent); */


        }

        [Route("savemargin")]
        [HttpPost]
        [Authorize(Roles = "BuyMarginMaker,SellMarginMaker")]
        public JsonResult SaveMargin([FromForm] Margin margin)
        {
            return new JsonResult(_cv.SaveMargin(margin, new Guid(HttpContext.User.FindFirst("id").Value)));
        }
        [Route("editMargin")]
        [HttpPut]
        [Authorize(Roles = "BuyMarginMaker,SellMarginMaker")]
        public JsonResult EditMargin([FromForm] Margin margin)
        {
            DataTable traderMargin = _db.GetMarginByTrader(margin.CFTTraderId);
            if (traderMargin.Rows.Count > 0)
            {
                if (Convert.ToInt16(traderMargin.Rows[0]["Status"]) == 2)
                {
                    return new JsonResult("Margin is already Approved");
                }
                if (Convert.ToInt16(traderMargin.Rows[0]["Status"]) == 3)
                {
                    return new JsonResult("Margin is already released");
                }
                return new JsonResult(_db.EditMargin(margin, new Guid(HttpContext.User.FindFirst("id").Value)) ? "OK" : "Error while releasing margin");
            }
            return new JsonResult("Unable to get margin");
        }
        [Route("updateStatus")]
        [HttpPut]
        [Authorize(Roles = "ApproveMargin")]
        public JsonResult ApproveReleaseMargin(Guid cftId, Guid cftTraderId)
        {
            return new JsonResult(_cv.ApproveMargin(cftId, cftTraderId, new Guid(HttpContext.User.FindFirst("id").Value)));
        }
        [Route("GetMarginByCftId")]
        [HttpGet]
        [Authorize(Roles = "MarginViewer")]
        public JsonResult GetMargin(Guid cftId)
        {
            return new JsonResult(_db.Getmargin(cftId));
        }
        [HttpGet("contractHisory")]
        [Authorize(Roles = "ContractViewer")]
        public JsonResult GetContractHistory(Guid id)
        {
            return new JsonResult(_db.GetContractHistoryById(id));
        }

        [HttpGet("colateralizedreport")]
        [Authorize(Roles = "ViewCollateralReport")]
        public JsonResult GetColateralizedReport()
        {
            return new JsonResult(_db.GetColateralizedReport());
        }

        [HttpPut("ReleaseMargin")]
        [Authorize(Roles = "ReleaseMargin")]
        public JsonResult ReleaseMargin(Guid cFTTraderId)
        {
            DataTable traderMargin = _db.GetMarginByTrader(cFTTraderId);
            if (traderMargin.Rows.Count > 0)
            {                
                if (Convert.ToInt16(traderMargin.Rows[0]["Status"]) == 3)
                {
                    return new JsonResult("Margin is already Released");
                }
                if (Convert.ToInt16(traderMargin.Rows[0]["Status"]) == 1)
                {
                    return new JsonResult("Margin should be approved");
                }
                new JsonResult(_db.ReleaseMargin(cFTTraderId, new Guid(HttpContext.User.FindFirst("id").Value)) ? "OK" : "Error while releasing margin");
           }
            return new JsonResult("Unable to get margin");
        }
    }
}
