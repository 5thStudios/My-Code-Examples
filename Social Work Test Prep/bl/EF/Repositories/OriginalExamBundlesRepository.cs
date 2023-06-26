using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using N.EntityFramework.Extensions;
using Umbraco.ModelsBuilder;

namespace Repositories
{
    public class OriginalExamBundlesRepository : IOriginalExamBundlesRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public OriginalExamBundlesRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public OriginalExamBundlesRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Select"
        public IEnumerable<Original_ExamBundles> GetAll()
        {
            return _dbContext.Original_ExamBundles.AsNoTracking();
        }
        #endregion



        #region "Add/Update"
        public void AddRecord(Original_ExamBundles Record)
        {
            _dbContext.Original_ExamBundles.Add(Record);
            Save();
        }
        public void BulkAddRecord(List<Original_ExamBundles> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void UpdateRecord(Original_ExamBundles Record)
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

