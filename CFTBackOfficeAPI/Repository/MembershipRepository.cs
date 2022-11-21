using CFTBackOfficeAPI.DataAccess;
using CFTBackOfficeAPI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Repository
{
    public class MembershipRepository : IMembership
    {
        private readonly IDataAccessProvider _db;
        private IConfiguration _cs;
        // private readonly IOptions<MyConfiguration> config;
        public MembershipRepository(IDataAccessProvider db, IConfiguration cs)
        {
            _cs = cs;
            _db = db;
        }
        
        public DataTable GetBusinessLiceness(Guid memberClientId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            paramName.Add("Id");
            ArrayList paramValue = new ArrayList();
            paramValue.Add(memberClientId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTGetBusinessLiceness", paramName, paramValue, ref erroMesg);
           
        }
        public DataTable GetAggremenet(Guid memberId,Guid clientId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            paramName.Add("MemberId");
            paramName.Add("ClientId");
            ArrayList paramValue = new ArrayList();
            paramValue.Add(memberId);
            paramValue.Add(clientId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTGetAgreementByClientId", paramName, paramValue, ref erroMesg);

        }
        public DataTable GetClentByMemberId(Guid memberId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            paramName.Add("memberId");
            ArrayList paramValue = new ArrayList();
            paramValue.Add(memberId);
            return  _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTGetAgreementByMemberId", paramName, paramValue, ref erroMesg);
            
        }
        public DataTable GetOption()
        {
            var erroMesg = "";
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "spCFTGetOption", ref erroMesg);
        }
        public DataTable GetMemberInformation()
        {
            var erroMesg = "";
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTGetAllMemberInformation", ref erroMesg);
           
        }
        public DataTable GetMarginLookUp()
        {
            var erroMesg = "";
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membership"], "dbo", "sp_CFTGetMarginLookup", ref erroMesg);

        }
    }
}
