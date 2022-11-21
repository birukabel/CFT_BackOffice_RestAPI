using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Services
{
    public interface ILookUp
    {
        public DataTable GetCommodity();
        public DataTable GetCommodityClassByCommodity(Guid commodity);
        public DataTable GetCommodityGradeByClassId(Guid classId);
        public DataTable GetWarehouse();
        public DataTable GetUserRights(Guid userId);
        public DataTable GetBankAccountByOwner(Guid TraderId);
    }
}
