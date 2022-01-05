using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using bl.EF;


namespace bl.Repositories
{
    public class ScrumActivityRepository : IScrumActivityRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public ScrumActivityRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public ScrumActivityRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public ScrumActivity GetRecord_byId(int Id)
        {
            return _dbContext.ScrumActivities.Where(x => x.CardId == Id).FirstOrDefault();
        }
        public IEnumerable<ScrumActivity> GetList_byCardId(int CardId)
        {
            return _dbContext.ScrumActivities.Where(x => x.CardId == CardId).OrderByDescending(x => x.CreatedTimestamp);
        }
        #endregion



        #region "Add"
        public void AddRecord(ScrumActivity Record)
        {
            _dbContext.ScrumActivities.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(ScrumActivity Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.ScrumActivities.Find(Id);
            if (Record != null) _dbContext.ScrumActivities.Remove(Record);
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

