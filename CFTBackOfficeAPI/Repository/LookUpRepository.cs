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
    public class LookUpRepository : ILookUp
    {
        private readonly IDataAccessProvider _db;
        private IConfiguration _cs;
        // private readonly IOptions<MyConfiguration> config;
        public LookUpRepository(IDataAccessProvider db, IConfiguration cs)
        {
            _cs = cs;
            _db = db;
        }

        public DataTable GetCommodity()
        {
            var erroMesg = "";
            return _db.ExecuteDataTable(_cs["ConnectionStrings:lookUp"], "dbo", "spGetCommodity", ref erroMesg);

        }

        public DataTable GetCommodityClassByCommodity(Guid commodityId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue=new ArrayList();
            paramName.Add("CommodityId");
            paramValue.Add(commodityId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:lookUp"], "dbo", "sp_CFTGetCommodtyClassByCommodity",paramName,paramValue, ref erroMesg);
        }

        public DataTable GetCommodityGradeByClassId(Guid classId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("CommodityClassGuid");
            paramValue.Add(classId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:lookUp"], "dbo", "spGetCommodityGradeByCommodityClass", paramName, paramValue, ref erroMesg);
        }

        public DataTable GetWarehouse()
        {
            var erroMesg = "";
            return _db.ExecuteDataTable(_cs["ConnectionStrings:lookUp"], "dbo", "spGetAllActiveWarehouse", ref erroMesg);
        }

        public DataTable GetUserRights(Guid userId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("@UserId");
            paramValue.Add(userId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:securityManager"], "dbo", "spCFTGetUserRights", paramName, paramValue, ref erroMesg);

        }
        public DataTable GetBankAccountByOwner(Guid TraderId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("@TraderId");
            paramValue.Add(TraderId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:cns"], "dbo", "spCFTGetBankAccountByOwner", paramName, paramValue, ref erroMesg);

        }
        

    }
}
