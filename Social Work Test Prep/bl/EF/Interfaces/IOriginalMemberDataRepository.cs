using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface IOriginalMemberDataRepository
    {
        List<Original_MemberData> SelectAll();
        int GetCount();
        Boolean DoesRecordExist(Original_MemberData memberData);
        List<string> GetAllEmails();
        int GetOldMemberId(string email);
        void AddRecord(Original_MemberData Record);
        void BulkAddRecord(List<Original_MemberData> LstRecords);
        void UpdateRecord(Original_MemberData Record);
    }
}

