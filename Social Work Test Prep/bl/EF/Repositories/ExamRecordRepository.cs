using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using bl.Models;
using N.EntityFramework.Extensions;
using Stripe;
using Umbraco.ModelsBuilder;

namespace Repositories
{
    public class ExamRecordRepository : IExamRecordRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public ExamRecordRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public ExamRecordRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Methods"
        public int GetCount()
        {
            return _dbContext.ExamRecords.AsNoTracking().Count();
        }
        public Boolean DoesRecordExist(ExamRecord examRecord)
        {
            return _dbContext.ExamRecords.Any(x =>
                                              x.MemberId == examRecord.MemberId &&
                                              x.ExamId == examRecord.ExamId &&
                                              x.CreatedDate == examRecord.CreatedDate);
        }
        #endregion



        #region "Select"
        public ExamRecord GetRecord_ById(int Id)
        {
            return _dbContext.ExamRecords.AsNoTracking().Where(x => x.ExamRecordId == Id).FirstOrDefault();
        }
        public List<ExamRecord> GetRecords_Unsubmitted_ByMemberId(int MemberId)
        {
            return _dbContext.ExamRecords.AsNoTracking().Where(x => x.MemberId == MemberId && !x.Submitted).ToList();
        }
        public List<ExamRecord> GetRecords_Submitted_ByMemberId_ExamModeId(int MemberId, int ExamModeId)
        {
            return _dbContext.ExamRecords.AsNoTracking().Where(x => x.MemberId == MemberId && x.ExamModeId == ExamModeId && x.Submitted).ToList();
        }

        public IEnumerable<ExamRecord> GetAll()
        {
            return _dbContext.ExamRecords.AsNoTracking();
        }
        public List<ExamRecord> GetAll_ByMemberId(int MemberId)
        {
            return _dbContext.ExamRecords.AsNoTracking().Where(x => x.MemberId == MemberId).ToList();
        }
        public HashSet<int> GetAllExamRecordIds_ByMemberId(int MemberId)
        {
            return _dbContext.ExamRecords.AsNoTracking().Where(x => x.MemberId == MemberId).Select(x => x.ExamRecordId).ToHashSet();
        }
        public HashSet<int?> GetAllDistinctIDs()
        {
            return _dbContext.ExamRecords.AsNoTracking().Where(x => x.MemberId != null).GroupBy(x => x.MemberId).Select(y => y.FirstOrDefault().MemberId ).ToHashSet();

            //List<int?> list = _dbContext.ExamRecords.AsNoTracking().Where(x => x.MemberId != null).Select(x => x.MemberId).ToList();
            //HashSet<int> hashset = new HashSet<int>();
            //var duplicates = list.Where(e => !hashset.Add((int)e));
            //return hashset;
        }
        public List<KeyValuePair> GetAll_Id_ExamId()
        {
            //Get only what we need from db as a generic list.
            var lst = _dbContext.ExamRecords.AsNoTracking()
                .Select(x => new
                {
                    ExamRecordId = x.ExamRecordId,
                    ExamId = x.ExamId

                }).ToList();

            //Convert list to class type and return
            return lst.Select(x => new KeyValuePair
            {
                IntKey = x.ExamRecordId,
                IntValue = x.ExamId

            }).ToList();
        }

        //case "StudyMode": //Obtain Submitted Study Mode Exams
        //    break;

        //case "TimedMode": //Obtain Submitted Exams
        //    break;

        //case "FreeMode": //Obtain Submitted Free Practice Exams

        #endregion



        #region "Add/Update"
        public void AddRecord(ExamRecord Record)
        {
            _dbContext.ExamRecords.Add(Record);
            Save();
        }
        public void UpdateRecord(ExamRecord Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }

        public void BulkAddRecord(List<ExamRecord> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkUpdateRecord(List<ExamRecord> LstRecords)
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

