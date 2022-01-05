using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using bl.EF;


namespace bl.Repositories
{
    public class ScrumCardRepository : IScrumCardRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public ScrumCardRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public ScrumCardRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public ScrumCard GetRecord_byId(int Id)
        {
            return _dbContext.ScrumCards.Where(x => x.CardId == Id).FirstOrDefault();
        }
        public IEnumerable<ScrumCard> GetList_byAccountId(int AccountId)
        {
            return _dbContext.ScrumCards.Where(x => x.AccountId == AccountId).OrderBy(x => x.StatusId).ThenBy(x => x.SortId);
        }
        #endregion



        #region "Add"
        public void AddRecord(ScrumCard Record)
        {
            _dbContext.ScrumCards.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(ScrumCard Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.ScrumCards.Find(Id);
            if (Record != null) _dbContext.ScrumCards.Remove(Record);
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

