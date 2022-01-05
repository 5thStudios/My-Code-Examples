using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IItemRepository
    {
        Item GetRecord_byId(int Id);
        IEnumerable<Item> GetList_byAccountId(int AccountId);
        IEnumerable<Item> GetList_byAccountId_ToolId(int AccountId, int ToolId);
        IEnumerable<string> GetList_ofCategories_byAccountId_ToolId(int AccountId, int ToolId);
        List<bl.Models.DuplicateItems> GetList_ofDuplicates_byAccountId_ToolId(int AccountId, int ToolId);
        List<bl.Models.DuplicateItems> GetList_ofDuplicatesInBugoutBags_byAccountId(int AccountId);


        void AddRecord(Item Record);
        void UpdateRecord(Item Record);
        void UpdateRecordsLocation(int Id, int? LocationId);
        void DeleteRecord(int Id);
        void DeleteLocation(int id);

        void Save();
    }
}

