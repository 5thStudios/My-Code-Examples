using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface ICmsMemberRepository
    {
        List<cmsMember> SelectAll();
        void AddRecord(cmsMember Record);
        void UpdateRecord(cmsMember Record);
        void BulkAddRecord(List<cmsMember> LstRecords);
        void BulkUpdateRecord(List<cmsMember> LstRecords);
    }
}