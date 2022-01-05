using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using bl.EF;


namespace bl.Repositories
{
    public class ScrumChecklistRepository : IScrumChecklistRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public ScrumChecklistRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public ScrumChecklistRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public ScrumChecklist GetRecord_byId(int Id)
        {
            return _dbContext.ScrumChecklists.Where(x => x.EntryId == Id).FirstOrDefault();
        }
        public IEnumerable<ScrumChecklist> GetList_byCardId(int CardId)
        {
            return _dbContext.ScrumChecklists.Where(x => x.CardId == CardId).OrderBy(x => x.SortOrder);
        }
        #endregion



        #region "Add"
        public void AddRecord(ScrumChecklist Record)
        {
            _dbContext.ScrumChecklists.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(ScrumChecklist Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.ScrumChecklists.Find(Id);
            if (Record != null) _dbContext.ScrumChecklists.Remove(Record);
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

