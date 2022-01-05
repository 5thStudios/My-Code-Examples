using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using MvcRepositoryDesignPattern_Demo.Models;
using System.Data.Entity;
using bl.EF;

namespace bl.Repositories
{
    public class PowerSourcesRepository : IPowerSourcesRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public PowerSourcesRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public PowerSourcesRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Methods"
        #endregion



        #region "Select"
        public PowerSource GetRecord_byId(int Id)
        {
            return _dbContext.PowerSources.Where(x => x.PowerSourceId == Id).FirstOrDefault();
        }
        #endregion



        #region "Add"
        public void AddRecord(PowerSource Record)
        {
            _dbContext.PowerSources.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(PowerSource Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.PowerSources.Find(Id);
            if (Record != null) _dbContext.PowerSources.Remove(Record);
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