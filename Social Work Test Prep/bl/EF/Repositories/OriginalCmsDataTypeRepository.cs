using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using N.EntityFramework.Extensions;
using Umbraco.ModelsBuilder;

namespace Repositories
{
    public class OriginalCmsDataTypeRepository : IOriginalCmsDataTypeRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public OriginalCmsDataTypeRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public OriginalCmsDataTypeRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Methods"
        //public Boolean DoesRecordExist(OriginalMemberData memberData)
        //{
        //    return _dbContext.OriginalMemberDatas.AsNoTracking().Any(x => x.Email == memberData.Email);
        //}
        #endregion



        #region "Add/Update"
        public void AddRecord(Original_CmsDataType Record)
        {
            _dbContext.Original_CmsDataType.Add(Record);
            Save();
        }
        public void BulkAddRecord(List<Original_CmsDataType> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void UpdateRecord(Original_CmsDataType Record)
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

