using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface IPageNotFoundRepository
    {
        List<SEOChecker_PageNotFound> GetAll();
        
        void AddRecord(SEOChecker_PageNotFound Record);
        void UpdateRecord(SEOChecker_PageNotFound Record);

        void BulkAddRecord(List<SEOChecker_PageNotFound> LstRecords);
        void BulkUpdateRecord(List<SEOChecker_PageNotFound> LstRecords);
    }
}