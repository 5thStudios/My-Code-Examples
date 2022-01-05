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
    public class FoodTypeRepository : IFoodTypeRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public FoodTypeRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public FoodTypeRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public FoodType GetRecord_byId(int Id)
        {
            return _dbContext.FoodTypes.Where(x => x.FoodTypeId == Id).FirstOrDefault();
        }
        public IEnumerable<FoodType> GetList()
        {
            return _dbContext.FoodTypes;
        }
        #endregion



        #region "Add"
        public void AddRecord(FoodType Record)
        {
            _dbContext.FoodTypes.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(FoodType Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.FoodTypes.Find(Id);
            if (Record != null) _dbContext.FoodTypes.Remove(Record);
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

