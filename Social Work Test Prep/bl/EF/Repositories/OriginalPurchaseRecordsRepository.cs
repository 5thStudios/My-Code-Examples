using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using N.EntityFramework.Extensions;
using Umbraco.ModelsBuilder;

namespace Repositories
{
    public class OriginalPurchaseRecordsRepository : IOriginalPurchaseRecordsRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public OriginalPurchaseRecordsRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public OriginalPurchaseRecordsRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Select"
        public Original_PurchaseRecords GetById(int Id)
        {
            return _dbContext.Original_PurchaseRecords.AsNoTracking().Where(x => x.id == Id).FirstOrDefault();
        }
        public List<Original_PurchaseRecords> GetAll()
        {
            return _dbContext.Original_PurchaseRecords.AsNoTracking().ToList();
        }
        public List<Original_PurchaseRecords> GetAll_ByTxnId(string TxnId)
        {
            return _dbContext.Original_PurchaseRecords.AsNoTracking().Where(x => x.txn_id == TxnId).ToList();
        }
        public List<Guid> GetAllGuids()
        {
            List<Guid> lstGuids = new List<Guid>();
            Guid guidOutput;

            //Parse guid list into actual list of guids
            var query = _dbContext.Original_PurchaseRecords.AsNoTracking().Select(x => x.originalId);
            foreach (var checkIfGuid in query)
            {
                if (checkIfGuid != null)
                {
                    if (Guid.TryParse(checkIfGuid.ToString(), out guidOutput)) lstGuids.Add(guidOutput);
                }
            }

            return lstGuids;
        }
        public List<string> GetAllTxnIDs()
        {
            List<string> lstTxnIDs = new List<string>();


            //
            var query = _dbContext.Original_PurchaseRecords.AsNoTracking().Select(x => x.txn_id);
            foreach (var txnId in query)
            {
                if (!string.IsNullOrEmpty(txnId))
                {
                    lstTxnIDs.Add(txnId);
                }
            }

            return lstTxnIDs;
        }
        #endregion


        #region "Add/Update"
        public void AddRecord(Original_PurchaseRecords Record)
        {
            _dbContext.Original_PurchaseRecords.Add(Record);
            Save();
        }
        public void UpdateRecord(Original_PurchaseRecords Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        public void BulkAddRecord(List<Original_PurchaseRecords> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkUpdateRecord(List<Original_PurchaseRecords> LstRecords)
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

