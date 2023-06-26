using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface IOriginalUpdateRecordRepository
    {
        List<Original_UpdateRecord> GetAll();

        void AddRecord(Original_UpdateRecord Record);
        void UpdateRecord(Original_UpdateRecord Record);

        void BulkAddRecord(List<Original_UpdateRecord> LstRecords);
        void BulkUpdateRecord(List<Original_UpdateRecord> LstRecords);
    }
}

