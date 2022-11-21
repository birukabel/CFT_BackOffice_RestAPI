using CFTBackOfficeAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Services
{
    public interface IContractSession
    {
        public bool Save(Guid createdBy);
        public bool Update(Guid id,int status, Guid updatedBy);
        public List<ContractSession> GetAllSession();
        public ContractSession GetSessionById(Guid Id);
        public DataTable GetSessionAuditTrial();
    }
}
