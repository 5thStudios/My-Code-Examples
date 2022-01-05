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
    public class LocationRepository : ILocationRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public LocationRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public LocationRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Methods"
        #endregion



        #region "Select"
        public Location GetRecord_byId(int Id)
        {
            return _dbContext.Locations.Where(x => x.LocationId == Id).FirstOrDefault();
        }
        public IEnumerable<Location> GetList_byAccountId(int AccountId)
        {
            return _dbContext.Locations.Where(x => x.AccountId == AccountId);
        }
        public IEnumerable<Location> GetBugoutBagList_byAccountId(int AccountId)
        {
            return _dbContext.Locations.Where(x => x.AccountId == AccountId && x.IsBugoutBag == true);
        }
        #endregion



        #region "Add"
        public void AddRecord(Location Record)
        {
            _dbContext.Locations.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(Location Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Location = _dbContext.Locations.Find(Id);
            if (Location != null) _dbContext.Locations.Remove(Location);
            Save();
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


    }
}

