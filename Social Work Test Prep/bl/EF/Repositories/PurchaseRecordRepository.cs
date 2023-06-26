using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using bl.EF;
using N.EntityFramework.Extensions;

namespace Repositories
{
    public class PurchaseRecordRepository : IPurchaseRecordRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public PurchaseRecordRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public PurchaseRecordRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion


      
        #region "Select"
        public List<PurchaseRecord> SelectAll()
        {
            return _dbContext.PurchaseRecords.AsNoTracking().ToList();
        }
        public List<PurchaseRecord> SelectAll_ByMemberId(int MemberId)
        {
            return _dbContext.PurchaseRecords.AsNoTracking().Where(x => x.MemberId == MemberId).ToList();
        }
        public PurchaseRecord Obtain_byId(int Id)
        {
            return _dbContext.PurchaseRecords.AsNoTracking().Where(x => x.PurchaseRecordId == Id).FirstOrDefault();
        }
        public Dictionary<int, DateTime> SelectAllPurchaseDates_asDictionary()
        {
            //Get only what we need from db as a generic list.
            var lst = _dbContext.PurchaseRecords.AsNoTracking()
                .Select(x => new
                {
                    PurchaseRecordId = x.PurchaseRecordId,
                    PurchaseDate = x.PurchaseDate

                }).ToList();


            //Convert list to dictionary and return
            return lst.ToDictionary(x => x.PurchaseRecordId, x => x.PurchaseDate);

        }
        public List<PurchaseRecord> ObtainAll_withCouponId()
        {
            return _dbContext.PurchaseRecords.AsNoTracking().Where(x => x.CouponId != null).ToList();
        }
        public PurchaseRecord SubmitPurchaseToSEO(int MemberId)
        {
            return _dbContext.PurchaseRecords.AsNoTracking().Where(x => x.MemberId == MemberId && !x.SubmittedToSEO).OrderByDescending(y => y.PurchaseDate).FirstOrDefault();
        }
        #endregion



        #region "Add"
        public void AddRecord(PurchaseRecord Record)
        {
            _dbContext.PurchaseRecords.Add(Record);
            Save();
        }
        public void UpdateRecord(PurchaseRecord Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        public void BulkAddRecord(List<PurchaseRecord> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkUpdateRecord(List<PurchaseRecord> LstRecords)
        {
            _dbContext.BulkUpdate(LstRecords, options => options.BatchSize = 1000);
        }
        #endregion



        //#region "Update"
        //public void UpdateRecord(ExamPurchase Record)
        //{
        //    //_dbContext.ExamPurchases(Record).State = EntityState.Modified;
        //    _dbContext.Set<ExamPurchase>().AddOrUpdate(Record);
        //    Save();
        //}
        //#endregion



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

