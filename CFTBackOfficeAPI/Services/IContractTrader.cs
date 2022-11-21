using CFTBackOfficeAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Services
{
    public interface IContractTrader
    {
        public bool Save(Guid createdBy, ContractTrader _contract);
        public bool Update(Contract ContractTrader, Guid updatedBy);
        public List<Contract> GetAllSession();
        public Contract GetSessionById(Guid Id);
        public DataTable GetSessionAuditTrial();
    }
}
