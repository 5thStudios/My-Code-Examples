using bl.EF;
using System.Collections.Generic;
using System;

namespace Repositories
{
    public interface IPurchaseRecordRepository
    {
        List<PurchaseRecord> SelectAll();
        List<PurchaseRecord> SelectAll_ByMemberId(int MemberId);
        PurchaseRecord Obtain_byId(int Id);
        List<PurchaseRecord> ObtainAll_withCouponId();
        Dictionary<int, DateTime> SelectAllPurchaseDates_asDictionary();
        PurchaseRecord SubmitPurchaseToSEO(int MemberId);

        void AddRecord(PurchaseRecord Record);
        void UpdateRecord(PurchaseRecord Record);
        void BulkAddRecord(List<PurchaseRecord> LstRecords);
        void BulkUpdateRecord(List<PurchaseRecord> LstRecords);
    }
}

