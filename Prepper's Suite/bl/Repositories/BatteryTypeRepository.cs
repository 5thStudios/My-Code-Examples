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
    public class BatteryTypeRepository : IBatteryTypeRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public BatteryTypeRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public BatteryTypeRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public BatteryType GetRecord_byId(int Id)
        {
            return _dbContext.BatteryTypes.Where(x => x.BatteryTypeId == Id).FirstOrDefault();
        }
        public IEnumerable<BatteryType> GetList()
        {
            return _dbContext.BatteryTypes;
        }
        #endregion



        #region "Add"
        public void AddRecord(BatteryType Record)
        {
            _dbContext.BatteryTypes.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(BatteryType Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.BatteryTypes.Find(Id);
            if (Record != null) _dbContext.BatteryTypes.Remove(Record);
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



        #region "Methods"
        #endregion
    }
}

