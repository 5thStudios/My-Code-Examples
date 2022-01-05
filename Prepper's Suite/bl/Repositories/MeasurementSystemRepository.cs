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
    public class MeasurementSystemRepository : IMeasurementSystemRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public MeasurementSystemRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public MeasurementSystemRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public MeasurementSystem GetRecord_byId(int Id)
        {
            return _dbContext.MeasurementSystems.Where(x => x.MeasurementSystemId == Id).FirstOrDefault();
        }
        public IEnumerable<MeasurementSystem> GetList()
        {
            return _dbContext.MeasurementSystems;
        }
        #endregion



        #region "Add"
        public void AddRecord(MeasurementSystem Record)
        {
            _dbContext.MeasurementSystems.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(MeasurementSystem Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.MeasurementSystems.Find(Id);
            if (Record != null) _dbContext.MeasurementSystems.Remove(Record);
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

