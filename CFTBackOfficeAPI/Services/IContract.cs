using CFTBackOfficeAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Services
{
    public interface IContract
    {
        public string Save(ContractDTO _contract,Guid CreatedBy);
        public string Update(ContractDTO _contract, Guid updatedBy);
        public DataTable GetAllContract();
        public List<ContractDetail> GetContractById(Guid Id);
        public DataTable GetDetailContract(Guid Id);
        public DataTable GetContractForAmmendment(Guid Id);
        public bool SaveAmmendment(ContractAmmendment _ammendment, Guid updatedBy);
        public int SaveMargin(Margin _margin, Guid createdBy);
        public bool ApproveReleaseMargin(Guid CFTTraderId, int status, Guid updatedBy);
        public bool EditMargin(Margin _margin, Guid updatedBy);
        public DataTable Getmargin(Guid cftId);
        public DataTable ContractHasMargin(Guid cftId);
        public DataTable GetMargin(Guid cftId);
        public DataTable GetMarginByTrader(Guid cftTraderId);
        public DataTable GetContractByCFTID(Guid cFTID);
        public DataTable GetContractHistoryById(Guid cFTID);
        public DataTable GetColateralizedReport();
        public bool ReleaseMargin(Guid cFTTraderId, Guid updatedBy);
       
    }
}
