using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Repositories
{
    public interface IMemberToolRepository
    {
        MemberTool GetRecord_byId(int Id);
        IEnumerable<MemberTool> GetList_byAccountId(int AccountId);

        void AddRecord(MemberTool Record);
        void UpdateRecord(MemberTool Record);

        Boolean DoesRecordExist(int AccountId, int ToolId);
        int GetCount_byAccountId(int AccountId);

        void Save();
    }
}