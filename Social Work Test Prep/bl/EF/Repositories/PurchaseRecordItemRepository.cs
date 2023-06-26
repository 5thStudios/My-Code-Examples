using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using bl.EF;
using N.EntityFramework.Extensions;

namespace Repositories
{
    public class PurchaseRecordItemRepository : IPurchaseRecordItemRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public PurchaseRecordItemRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public PurchaseRecordItemRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Methods"
        public Boolean DoesRecordExists_byMemberId_ExamId(int MemberId, int ExamId)
        {
            return _dbContext.PurchaseRecordItems.AsNoTracking().Where(x => x.MemberId == MemberId && x.ExamId == ExamId).Any();
        }
        #endregion



        #region "Select"
        public PurchaseRecordItem ObtainRecord_byId(int Id)
        {
            return _dbContext.PurchaseRecordItems.AsNoTracking().FirstOrDefault(x => x.PurchaseRecordItemId == Id);
        }
        public List<PurchaseRecordItem> ObtainRecords_byPurchaseRecordId(int PurchaseRecordId)
        {
            return _dbContext.PurchaseRecordItems.AsNoTracking().Where(x => x.PurchaseRecordId == PurchaseRecordId).ToList();
        }
        public List<PurchaseRecordItem> SelectAll()
        {
            return _dbContext.PurchaseRecordItems.AsNoTracking().ToList();
        }
        public List<PurchaseRecordItem> ObtainRecords_byMemberId(int MemberId)
        {
            return _dbContext.PurchaseRecordItems.AsNoTracking().Where(x => x.MemberId == MemberId).ToList();
        }
        public PurchaseRecordItem ObtainRecord_byMemberId_ExamId(int MemberId, int ExamId)
        {
            return _dbContext.PurchaseRecordItems.AsNoTracking().Where(x => x.MemberId == MemberId && x.ExamId == ExamId).OrderByDescending(q => q.ExpirationDate).FirstOrDefault();
        }
        #endregion



        #region "Add"
        public void AddRecord(PurchaseRecordItem Record)
        {
            _dbContext.PurchaseRecordItems.Add(Record);
            Save();
        }
        public void BulkAddRecord(List<PurchaseRecordItem> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        #endregion

        #region "Update"
        public void UpdateRecord(PurchaseRecordItem Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        public void BulkUpdateRecord(List<PurchaseRecordItem> LstRecords)
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

