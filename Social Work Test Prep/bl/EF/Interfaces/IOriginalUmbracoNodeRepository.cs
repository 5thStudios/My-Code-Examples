using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface IOriginalUmbracoNodeRepository
    {
        HashSet<int> GetAllIDs();
        IEnumerable<Original_UmbracoNode> SelectAll_Lvl4();
        IEnumerable<Original_UmbracoNode> SelectAll_Lvl4_ByEmail(string email);
        IEnumerable<Original_UmbracoNode> SelectAll_Lvl5();
        IEnumerable<Original_UmbracoNode> SelectAll_Lvl6();
        IEnumerable<Original_UmbracoNode> SelectAll_Lvl7();

        void AddRecord(Original_UmbracoNode Record);
        void UpdateRecord(Original_UmbracoNode Record);

        void BulkAddRecord(List<Original_UmbracoNode> LstRecords);
        void BulkUpdateRecord(List<Original_UmbracoNode> LstRecords);
        void BulkDeleteRecords(List<Original_UmbracoNode> LstRecords);
    }
}

