using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface IOriginalPurchaseRecordsRepository
    {
        Original_PurchaseRecords GetById(int Id);
        List<Original_PurchaseRecords> GetAll();
        List<Original_PurchaseRecords> GetAll_ByTxnId(string TxnId);
        List<Guid> GetAllGuids();
        List<string> GetAllTxnIDs();

        void AddRecord(Original_PurchaseRecords Record);
        void UpdateRecord(Original_PurchaseRecords Record);

        void BulkAddRecord(List<Original_PurchaseRecords> LstRecords);
        void BulkUpdateRecord(List<Original_PurchaseRecords> LstRecords);
    }
}

