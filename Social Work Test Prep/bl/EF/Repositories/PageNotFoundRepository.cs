using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using N.EntityFramework.Extensions;
using SEOChecker.Core.Repository.PageNotFound;

namespace Repositories
{
    public class PageNotFoundRepository : IPageNotFoundRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public PageNotFoundRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public PageNotFoundRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Select"
        public List<SEOChecker_PageNotFound> GetAll()
        {
            return _dbContext.SEOChecker_PageNotFound.AsNoTracking().ToList();
        }
        #endregion



        #region "Add/Update"
        public void AddRecord(SEOChecker_PageNotFound Record)
        {
            _dbContext.SEOChecker_PageNotFound.Add(Record);
            Save();
        }
        public void UpdateRecord(SEOChecker_PageNotFound Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }

        public void BulkAddRecord(List<SEOChecker_PageNotFound> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkUpdateRecord(List<SEOChecker_PageNotFound> LstRecords)
        {
            _dbContext.BulkUpdate(LstRecords, options => options.BatchSize = 1000);
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

