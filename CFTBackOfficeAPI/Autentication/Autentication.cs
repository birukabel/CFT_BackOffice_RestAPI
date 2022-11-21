using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using CFTBackOfficeAPI.Services;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Extensions.Configuration;
using ECXSecurity;
using Microsoft.Owin;
using System.Security.Claims;
using CFTBackOfficeAPI.Models;
using System.Text;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
//using Microsoft.AspNetCore.Http;

namespace CFTBackOfficeAPI.Autentication
{
    public class Autentication : IAutentication//OAuthAuthorizationServerProvider
    {
        static User user;
        readonly IConfiguration _cs;
        readonly IECXSecurityService _securityService;
        readonly ILookUp _il;
        public Autentication(IConfiguration cs, IECXSecurityService securityService, ILookUp il)
        {
            _cs = cs;
            _securityService = securityService;
            _il = il;
        }

        public async Task<string> AutenticateUser(string userName, string password)
        {
            if (AutenticateToAD(_cs["DirPath"], _cs["domain"], userName, password, _cs["ACDUser"], _cs["ACDPass"]))
            {
                ECXSecurityAccessSoapClient client = _securityService.GetService();
                IsAuthenticatedResponse AuStatus = await client.IsAuthenticatedAsync(userName, password, "CFT");

                if (AuStatus.Body.IsAuthenticatedResult == AuthenticationStatus.AccessGranted)
                {
                    return CreateToken();
                }
                return "You don't have access to this application";
            }
            return "Invalid username or password";
        }

        //public async Task<string> CheckUserHasRight(string userId, ArrayOfString code)
        //{
        //    //ECXSecurityAccessSoapClient client = _securityService.GetService();
        //    //Task<HasRightsResponse> response = client.HasRightsAsync(userId, code,"");
        //    //return response.Status == 2 ? @"User doesn't have access right" : "User has access right";
        //}

        bool AutenticateToAD(string dirPath, string _domain, string userName, string pwd, string _adAdminUser, string _adAdminPass)
        {
            string domain = _domain;//SettingReader.ADDomainNameForEmployees;
            string LDAP_Path = dirPath;//SettingReader.ADPathForEmployees
            //string container = "OU=Trade, DC=Trade, DC=ECX, DC=com";
            string adAdminUser = _adAdminUser;//System.Configuration.ConfigurationManager.AppSettings["ACDUser"];
            string adAdminPass = _adAdminPass;//System.Configuration.ConfigurationManager.AppSettings["ACDPass"];

            if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(LDAP_Path))
                return false;
            string domainAndUsername = domain + "\\" + userName;

            try
            {
                #region Authenticate using Directory Search
                //DirectoryEntry entry = new DirectoryEntry(LDAP_Path, userName, pwd, AuthenticationTypes.Secure | AuthenticationTypes.Sealing);
                using (System.DirectoryServices.DirectoryEntry entry = new(LDAP_Path, userName, pwd, AuthenticationTypes.Secure | AuthenticationTypes.Sealing | AuthenticationTypes.Signing))
                {
                    //Bind to the native AdsObject to force authentication.
                    object obj = entry.NativeObject;
                    DirectorySearcher search = new DirectorySearcher(entry);

                    search.Filter = "(sAMAccountName=" + userName + ")";
                    search.PropertiesToLoad.Add("CN");
                    SearchResultCollection results = search.FindAll();
                    if (results == null || results.Count == 0)
                    {//no AD record found
                        return false;
                    }
                    if (results.Count > 1)
                    {//multiple AD records were found
                        results.Dispose();
                        return false;
                    }
                    SearchResult result = results[0];//take the first AD Record

                    if (result != null)
                    {
                        System.DirectoryServices.DirectoryEntry userADEntry = result.GetDirectoryEntry();
                        user = new();
                        user.ID = userADEntry.Guid;
                        user.Name = userADEntry.Username;
                        //Session["LoggedUser"] = userADID;
                    }
                    else
                    {
                        return false;
                    }
                    entry.Close();
                    return true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                return false;//authentication fails - let the AD handle the # of trials

                //throw new Exception("Error authenticating user. \n" + ex.Message);
            }
        }

        string CreateToken()
        {
            List<Claim> claims = new()
            {
                new(ClaimTypes.Name, user.Name),
                new("Id", user.ID.ToString())                
            };
            List<string> userRrights = new();
            DataTable dtUserRoles = _il.GetUserRights(user.ID);
            if (dtUserRoles.Rows.Count > 0)
            {
                //ContractViewer
                //ContractChecker
                //ContractMaker
                //SellMarginMaker
                //SellMarginChecker
                //BuyMarginChecker
                //BuyMarginMaker
                //SessionTemplateChk
                //SessionStatus
                foreach (DataRow dr in dtUserRoles.Rows)
                {
                    claims.Add(new("role", dr["RightCode"].ToString()));
                }
            }
           
            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cs["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _cs["Jwt:Issuer"],
                _cs["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signIn);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

       
    }
}
