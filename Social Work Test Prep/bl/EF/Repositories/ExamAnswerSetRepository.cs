using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;


namespace Repositories
{
    public class ExamAnswerSetRepository : IExamAnswerSetRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public ExamAnswerSetRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public ExamAnswerSetRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Methods"
        public int GetCount()
        {
            return _dbContext.ExamAnswerSets.AsNoTracking().Count();
        }
        #endregion



        #region "Select"
        public ExamAnswerSet GetRecord_ById(int Id)
        {
            return _dbContext.ExamAnswerSets.AsNoTracking().Where(x => x.ExamAnswerSetId == Id).FirstOrDefault();
        }
        public ExamAnswerSet GetRecord_ByExamRecordId(int Id)
        {
            return _dbContext.ExamAnswerSets.AsNoTracking().Where(x => x.ExamRecordId == Id).FirstOrDefault();
        }
        public IEnumerable<ExamAnswerSet> GetAll()
        {
            return _dbContext.ExamAnswerSets.AsNoTracking();
        }
        #endregion



        #region "Add"
        public void AddRecord(ExamAnswerSet Record)
        {
            _dbContext.ExamAnswerSets.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(ExamAnswerSet Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
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

