using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using N.EntityFramework.Extensions;
using Umbraco.ModelsBuilder;

namespace Repositories
{
    public class OriginalMemberDataRepository : IOriginalMemberDataRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public OriginalMemberDataRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public OriginalMemberDataRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Methods"
        public int GetCount()
        {
            return _dbContext.Original_MemberData.AsNoTracking().Count();
        }
        public Boolean DoesRecordExist(Original_MemberData memberData)
        {
            return _dbContext.Original_MemberData.AsNoTracking().Any(x => x.Email == memberData.Email);
        }
        public List<string> GetAllEmails()
        {
            return _dbContext.Original_MemberData.AsNoTracking().Select(x => x.Email).ToList();
        }
        #endregion



        #region "Selects"
        public List<Original_MemberData> SelectAll()
        {
            return _dbContext.Original_MemberData.AsNoTracking().ToList();
        }
        public int GetOldMemberId(string email)
        {
            return _dbContext.Original_MemberData.AsNoTracking().Where(x => x.Email == email).FirstOrDefault().MemberId;
        }
        #endregion



        #region "Add/Update"
        public void AddRecord(Original_MemberData Record)
        {
            _dbContext.Original_MemberData.Add(Record);
            Save();
        }
        public void BulkAddRecord(List<Original_MemberData> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void UpdateRecord(Original_MemberData Record)
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

