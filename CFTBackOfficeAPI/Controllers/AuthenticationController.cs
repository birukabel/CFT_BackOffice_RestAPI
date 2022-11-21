using Microsoft.AspNetCore.Mvc;
using System;
using CFTBackOfficeAPI.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace CFTBackOfficeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAutentication _autentication;
        public AuthenticationController(IAutentication autentication)
        {
            _autentication = autentication;
        }
        [HttpGet("autenticate")]
       
        public async Task<ActionResult> Login(string userName, string password)
        {
            var saron_jil_felefelex_anotherjil =  await _autentication.AutenticateUser(userName, password);
            return Ok(saron_jil_felefelex_anotherjil);
        }

        [HttpGet("logout")]
        public ActionResult LogOut()
        {
            HttpContextAccessor httpContextAccessor = new();

           // httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Remove(0);
            return Ok();
        }
       
    }
}
