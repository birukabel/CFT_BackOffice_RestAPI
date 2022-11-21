using CFTBackOfficeAPI.DataAccess;
using CFTBackOfficeAPI.Models;
using CFTBackOfficeAPI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Repository
{
    public class SessionRepository : IContractSession
    {
        private readonly IDataAccessProvider _db;
        private IConfiguration _cs;
        // private readonly IOptions<MyConfiguration> config;
        public SessionRepository(IDataAccessProvider db, IConfiguration cs)
        {
            _cs = cs;
            _db = db;
        }

        public List<ContractSession> GetAllSession()
        {
            var erroMesg = "";
            DataTable data = _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "sp_CFTGetAllSession", ref erroMesg);
            return _db.ConvertDataTable<ContractSession>(data);

        }

        public ContractSession GetSessionById(Guid Id)
        {
            throw new NotImplementedException();
        }
        public DataTable GetSessionAuditTrial()
        {
            var erroMesg = "";
            return _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "sp_CFTGettblSessionAuditTrail", ref erroMesg);
        }

        public bool Save(Guid createdBy)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            paramName.Add("CreatedBy");
            ArrayList paramValue = new ArrayList();
            paramValue.Add(createdBy);
            return _db.ExecuteNonQuery(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "sp_CFTSessionSave", paramName, paramValue, ref erroMesg);

        }

        public bool Update(Guid id,int status, Guid updatedBy)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            paramName.Add("ID");
            paramName.Add("Status");
            paramName.Add("UpdatedBy");
            ArrayList paramValue = new ArrayList();
            paramValue.Add(id);
            paramValue.Add(status);
            paramValue.Add(updatedBy);
            return _db.ExecuteNonQuery(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "sp_CFTSessionUpdate", paramName, paramValue, ref erroMesg);
        }

        
    }
}
