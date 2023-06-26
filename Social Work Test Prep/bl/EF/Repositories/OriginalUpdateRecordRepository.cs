using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using N.EntityFramework.Extensions;
using Umbraco.ModelsBuilder;

namespace Repositories
{
    public class OriginalUpdateRecordRepository : IOriginalUpdateRecordRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public OriginalUpdateRecordRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public OriginalUpdateRecordRepository(EF_SwtpDb context)
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



        #region "Selects"
        public List<Original_UpdateRecord> GetAll()
        {
            return _dbContext.Original_UpdateRecord.AsNoTracking().ToList();
        }
        #endregion



        #region "Add/Update"
        public void AddRecord(Original_UpdateRecord Record)
        {
            _dbContext.Original_UpdateRecord.Add(Record);
            Save();
        }
        public void UpdateRecord(Original_UpdateRecord Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        public void BulkAddRecord(List<Original_UpdateRecord> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkUpdateRecord(List<Original_UpdateRecord> LstRecords)
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

