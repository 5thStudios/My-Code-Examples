using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface IOriginalCmsDataTypeRepository
    {
        void AddRecord(Original_CmsDataType Record);
        void BulkAddRecord(List<Original_CmsDataType> LstRecords);
        void UpdateRecord(Original_CmsDataType Record);
    }
}

