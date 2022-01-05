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
    public class SeasonRepository : ISeasonRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public SeasonRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public SeasonRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public Season GetRecord_byId(int Id)
        {
            return _dbContext.Seasons.Where(x => x.SeasonId == Id).FirstOrDefault();
        }
        public IEnumerable<Season> GetList()
        {
            return _dbContext.Seasons;
        }
        #endregion



        #region "Add"
        public void AddRecord(Season Record)
        {
            _dbContext.Seasons.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(Season Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.Seasons.Find(Id);
            if (Record != null) _dbContext.Seasons.Remove(Record);
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

