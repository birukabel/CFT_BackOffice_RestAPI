using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFTBackOfficeAPI.Services
{
    public interface IMembership
    {
        public DataTable GetMemberInformation();
        public DataTable GetClentByMemberId(Guid memberId);
        public DataTable GetBusinessLiceness(Guid memberClientId);
        public DataTable GetAggremenet(Guid memberId,Guid clientId);
        public DataTable GetOption();
        public DataTable GetMarginLookUp();
    }
}
