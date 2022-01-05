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
    public class FuelTypeRepository : IFuelTypeRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public FuelTypeRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public FuelTypeRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public FuelType GetRecord_byId(int Id)
        {
            return _dbContext.FuelTypes.Where(x => x.FuelTypeId == Id).FirstOrDefault();
        }
        public IEnumerable<FuelType> GetList()
        {
            return _dbContext.FuelTypes;
        }
        #endregion



        #region "Add"
        public void AddRecord(FuelType Record)
        {
            _dbContext.FuelTypes.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(FuelType Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.FuelTypes.Find(Id);
            if (Record != null) _dbContext.FuelTypes.Remove(Record);
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

