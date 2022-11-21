using CFTBackOfficeAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Services
{
    public interface IContractValidator
    {
        public  Task<string> SaveMargin(Margin marign,Guid createdBy);
        public Task<string> ApproveMargin(Guid cftId, Guid cftTraderId, Guid approvedBy);
    }
}
