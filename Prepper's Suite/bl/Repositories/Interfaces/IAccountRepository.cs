using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Repositories
{
    public interface IAccountRepository
    {
        Boolean DoesRecordExist(int MemberId);

        Account GetRecord_byMemberId(int MemberId);

        void AddRecord(Account Record);

        void Save();
    }
}
