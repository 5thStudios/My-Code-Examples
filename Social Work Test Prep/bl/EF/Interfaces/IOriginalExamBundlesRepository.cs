using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface IOriginalExamBundlesRepository
    {
        IEnumerable<Original_ExamBundles> GetAll();
        void AddRecord(Original_ExamBundles Record);
        void BulkAddRecord(List<Original_ExamBundles> LstRecords);
        void UpdateRecord(Original_ExamBundles Record);
    }
}

