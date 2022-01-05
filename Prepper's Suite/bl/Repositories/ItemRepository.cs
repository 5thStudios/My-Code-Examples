using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using MvcRepositoryDesignPattern_Demo.Models;
using System.Data.Entity;
using bl.EF;
using System.Web.Mvc;

namespace bl.Repositories
{
    public class ItemRepository : IItemRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public ItemRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public ItemRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public Item GetRecord_byId(int Id)
        {
            return _dbContext.Items.Where(x => x.ItemId == Id).FirstOrDefault();
        }
        public IEnumerable<Item> GetList_byAccountId(int AccountId)
        {
            return _dbContext.Items.Where(x => x.AccountId == AccountId).OrderBy(x => x.Name);
        }
        public IEnumerable<Item> GetList_byAccountId_ToolId(int AccountId, int ToolId)
        {
            return _dbContext.Items.Where(x => x.AccountId == AccountId && x.ToolId == ToolId).OrderBy(x => x.Category.Name).ThenBy(x => x.Name);
        }
        public IEnumerable<string> GetList_ofCategories_byAccountId_ToolId(int AccountId, int ToolId)
        {
            return _dbContext.Items.Where(x => x.AccountId == AccountId && x.ToolId == ToolId).Select(x => x.Category.Name).Distinct().OrderBy(x => x);
        }
        public List<bl.Models.DuplicateItems> GetList_ofDuplicates_byAccountId_ToolId(int AccountId, int ToolId)
        {
            //Get initial list of all items for parameters.
            var LstItems = _dbContext.Items.Where(x => x.AccountId == AccountId && x.ToolId == ToolId).OrderBy(x => x.Category.Name).ThenBy(x => x.Name);

            //Return list of all duplicates from list.
            return LstItems.GroupBy(x => new { x.Category, x.Name })
              .Where(y => y.Count() > 1)
              .Select(z => new bl.Models.DuplicateItems
              {
                  Category = z.Key.Category,
                  ItemName = z.Key.Name,
                  Count = z.Count(),
                  LstItems = z.ToList()
              })
              .ToList();
        }
        public List<bl.Models.DuplicateItems> GetList_ofDuplicatesInBugoutBags_byAccountId(int AccountId)
        {
            //Get initial list of all items for parameters.
            var LstItems = _dbContext.Items.Where(x => x.AccountId == AccountId && x.Location.IsBugoutBag == true).OrderBy(x => x.Category.Name).ThenBy(x => x.Name);
           // var LstItems = _dbContext.Items.Where(x => x.AccountId == AccountId && x.ToolId == ToolId).OrderBy(x => x.Category.Name).ThenBy(x => x.Name);

            //Return list of all duplicates from list.
            return LstItems.GroupBy(x => new { x.Location, x.Name })
              .Where(y => y.Count() > 1)
              .Select(z => new bl.Models.DuplicateItems
              {
                  Location = z.Key.Location,
                  ItemName = z.Key.Name,
                  Count = z.Count(),
                  LstItems = z.ToList()
              })
              .ToList();
        }
        #endregion



        #region "Add"
        public void AddRecord(Item Record)
        {
            _dbContext.Items.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(Item Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        public void UpdateRecordsLocation(int Id, int? LocationId)
        {
            Item Record =  _dbContext.Items.Where(x => x.ItemId == Id).FirstOrDefault();
            Record.LocationId = LocationId;
            Record.LastUpdatedTimestamp = DateTime.Now;
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Item = _dbContext.Items.Find(Id);
            if (Item != null) _dbContext.Items.Remove(Item);
            Save();
        }
        public void DeleteLocation(int id)
        {
            //
            List<Item> LstItems = _dbContext.Items.Where(x => x.LocationId == id).ToList();
            foreach (Item Record in LstItems)
            {
                Record.LocationId = null;
                UpdateRecord(Record);
            }
        }
        #endregion



        #region "Save & Dispose"
        public void Save()
        {
            _dbContext.SaveChanges();
        }


        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion



        #region "Methods"
        #endregion
    }
}

