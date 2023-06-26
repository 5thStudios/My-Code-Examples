using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface IOriginalCmsPropertyTypeRepository
    {
        void AddRecord(Original_CmsPropertyType Record);
        void BulkAddRecord(List<Original_CmsPropertyType> LstRecords);
        void UpdateRecord(Original_CmsPropertyType Record);
    }
}

