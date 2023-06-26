using bl.EF;
using System.Collections.Generic;
using System;

namespace Repositories
{
    public interface IPurchaseRecordItemRepository
    {
        Boolean DoesRecordExists_byMemberId_ExamId(int MemberId, int ExamId);

        PurchaseRecordItem ObtainRecord_byId(int Id);
        List<PurchaseRecordItem> ObtainRecords_byPurchaseRecordId(int PurchaseRecordId);
        List<PurchaseRecordItem> SelectAll();
        List<PurchaseRecordItem> ObtainRecords_byMemberId(int MemberId);
        PurchaseRecordItem ObtainRecord_byMemberId_ExamId(int MemberId, int ExamId);

        void AddRecord(PurchaseRecordItem Record);
        void UpdateRecord(PurchaseRecordItem Record);

        void BulkAddRecord(List<PurchaseRecordItem> LstRecords);
        void BulkUpdateRecord(List<PurchaseRecordItem> LstRecords);
    }
}

