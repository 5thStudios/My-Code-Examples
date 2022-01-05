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
    public class MemberToolRepository : IMemberToolRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public MemberToolRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public MemberToolRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Methods"
        public Boolean DoesRecordExist(int AccountId, int ToolId)
        {
            return _dbContext.MemberTools.Any(x => x.AccountId == AccountId && x.ToolId == ToolId);
        }
        public int GetCount_byAccountId(int AccountId)
        {
            return _dbContext.MemberTools.Where(x => x.AccountId == AccountId).Count();
        }
        #endregion



        #region "Select"
        public IEnumerable<MemberTool> GetList_byAccountId(int AccountId)
        {
            return _dbContext.MemberTools.Where(x => x.AccountId == AccountId);
        }
        public MemberTool GetRecord_byId(int Id)
        {
            return _dbContext.MemberTools.Where(x => x.MemberToolId == Id).FirstOrDefault();
        }
        #endregion



        #region "Add"
        public void AddRecord(MemberTool Record)
        {
            _dbContext.MemberTools.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(MemberTool Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
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