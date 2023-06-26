using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Configuration;
using bl.EF;
using N.EntityFramework.Extensions;
using Umbraco.ModelsBuilder;

namespace Repositories
{
    public class OriginalCmsPropertyDataRepository : IOriginalCmsPropertyDataRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public OriginalCmsPropertyDataRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public OriginalCmsPropertyDataRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Selects"
        public HashSet<int> SelectAllIDs()
        {
            return _dbContext.Original_CmsPropertyData.AsNoTracking().Select(x => x.id).ToHashSet();
        }
        public IEnumerable<Original_CmsPropertyData> SelectAll()
        {
            return _dbContext.Original_CmsPropertyData.AsNoTracking();
        }
        public IEnumerable<Original_CmsPropertyData> SelectAll_byContentNodeId(int ContentNodeId)
        {
            return _dbContext.Original_CmsPropertyData.AsNoTracking().Where(x => x.contentNodeId == ContentNodeId);
        }
        public Original_CmsPropertyData Select_byContentNodeId_PropertyTypeId(int ContentNodeId, int PropertyTypeId)
        {
            return _dbContext.Original_CmsPropertyData.AsNoTracking().FirstOrDefault(x => x.contentNodeId == ContentNodeId && x.propertytypeid == PropertyTypeId);
        }
        #endregion



        #region "Add/Update"
        public void AddRecord(Original_CmsPropertyData Record)
        {
            _dbContext.Original_CmsPropertyData.Add(Record);
            Save();
        }
        public void UpdateRecord(Original_CmsPropertyData Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }

        public void BulkAddRecord(List<Original_CmsPropertyData> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkUpdateRecord(List<Original_CmsPropertyData> LstRecords)
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

