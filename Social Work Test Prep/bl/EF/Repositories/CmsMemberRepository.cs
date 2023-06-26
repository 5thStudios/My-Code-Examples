using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using N.EntityFramework.Extensions;
using SEOChecker.UI.Model.Fields;


namespace Repositories
{
    public class CmsMemberRepository : ICmsMemberRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public CmsMemberRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public CmsMemberRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Select"
        public List<cmsMember> SelectAll()
        {
            return _dbContext.cmsMembers.AsNoTracking().ToList();
        }
        //public List<cmsMember> SelectAllDuplicates()
        //{
        //    //var duplicates = listOfItems
        //    //.GroupBy(i => i)
        //    //.Where(g => g.Count() > 1)
        //    //.Select(g => g.Key);

        //    var a = _dbContext.cmsMembers.AsNoTracking().ToList();

        //    var b = _dbContext.cmsMembers.AsNoTracking().GroupBy(x => x.LoginName).Where(y => y.Count() > 1);


        //    return null;
        //}
        #endregion


        #region "Add/Update"
        public void AddRecord(cmsMember Record)
        {
            _dbContext.cmsMembers.Add(Record);
            Save();
        }
        public void UpdateRecord(cmsMember Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }

        public void BulkAddRecord(List<cmsMember> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkUpdateRecord(List<cmsMember> LstRecords)
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

