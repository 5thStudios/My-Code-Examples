using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface IOriginalCmsPropertyDataRepository
    {
        HashSet<int> SelectAllIDs();
        IEnumerable<Original_CmsPropertyData> SelectAll();
        IEnumerable<Original_CmsPropertyData> SelectAll_byContentNodeId(int ContentNodeId);
        Original_CmsPropertyData Select_byContentNodeId_PropertyTypeId(int ContentNodeId, int PropertyTypeId);

        void AddRecord(Original_CmsPropertyData Record);
        void UpdateRecord(Original_CmsPropertyData Record);

        void BulkAddRecord(List<Original_CmsPropertyData> LstRecords);
        void BulkUpdateRecord(List<Original_CmsPropertyData> LstRecords);
    }
}

