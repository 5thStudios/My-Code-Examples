using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using MvcRepositoryDesignPattern_Demo.Models;
using System.Data.Entity;
using bl.EF;

namespace bl.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public AccountRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public AccountRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Methods"
        public Boolean DoesRecordExist(int MemberId)
        {
            return _dbContext.Accounts.Any(x => x.AccountHolderId == MemberId);
        }
        #endregion



        #region "Select"
        public Account GetRecord_byMemberId(int MemberId)
        {
            return _dbContext.Accounts.Where(x => x.AccountHolderId == MemberId).FirstOrDefault();
        }
        #endregion



        #region "Add"
        public void AddRecord(Account Record)
        {
            _dbContext.Accounts.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        #endregion



        #region "Delete"
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