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
    public class MemberRepository : IMemberRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public MemberRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public MemberRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Methods"
        public Boolean DoesAccountHolderExist_byAccountId(int AccountId)
        {
            return _dbContext.Members.Any(x => x.AccountId == AccountId && x.IsAccountOwner == true);
        }
        #endregion



        #region "Select"
        public Member GetRecord_byAccountId(int AccountId)
        {
            return _dbContext.Members.Where(x => x.AccountId == AccountId && x.IsAccountOwner == true).FirstOrDefault();
        }
        public Member GetRecord_byMemberId(int Id)
        {
            return _dbContext.Members.Where(x => x.MemberId == Id).FirstOrDefault();
        }
        public IEnumerable<Member> GetList_byAccountId(int AccountId)
        {
            return _dbContext.Members.Where(x => x.AccountId == AccountId);
        }
        #endregion



        #region "Add"
        public void AddRecord(Member Record)
        {
            _dbContext.Members.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(Member Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Member = _dbContext.Members.Find(Id);
            if (Member != null) _dbContext.Members.Remove(Member);
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



//public void AddNewMember(Member Member)
//{
//    _dbContext.Members.Add(Member);
//    Save();
//}
//public void UpdateMember(Member Member)
//{
//    _dbContext.Entry(Member).State = EntityState.Modified;
//}

//public void DeleteMember(int id)
//{
//    var Member = _dbContext.Members.Find(id);
//    if (Member != null) _dbContext.Members.Remove(Member);
//}


//public IEnumerable<Member> GetMembers()
//{
//    return _dbContext.Members.ToList();
//}

//public Boolean DoesMemberExist_ByAccountId(int AccountId)
//{
//    return _dbContext.Members.Any(x => x.AccountId == AccountId);
//}