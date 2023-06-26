using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using bl.EF;
using bl.Models;
using N.EntityFramework.Extensions;

namespace Repositories
{
    public class ExamAnswerRepository : IExamAnswerRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public ExamAnswerRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public ExamAnswerRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Methods"
        public int GetCount()
        {
            return _dbContext.ExamAnswers.AsNoTracking().Count();
        }

        public int GetCount_ByExamAnswerSetId(int ExamAnswerSetId)
        {
            return _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswerSetId == ExamAnswerSetId).Count();
        }
        public int GetCount_ByExamAnswerSetId_CorrectAnswers(int ExamAnswerSetId)
        {
            return _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswerSetId == ExamAnswerSetId && x.IsCorrect).Count();
        }
        public int GetCount_ByExamAnswerSetId_IncorrectAnswers(int ExamAnswerSetId)
        {
            return _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswerSetId == ExamAnswerSetId && !x.IsCorrect).Count();
        }

        public int GetCount_ByExamAnswerSetId_ContentAreaId(int ExamAnswerSetId, int ContentAreaId)
        {
            return _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswerSetId == ExamAnswerSetId && x.ContentAreaId == ContentAreaId).Count();
        }
        public int GetCount_ByExamAnswerSetId_ContentAreaId_CorrectAnswer(int ExamAnswerSetId, int ContentAreaId)
        {
            return _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswerSetId == ExamAnswerSetId && x.ContentAreaId == ContentAreaId && x.IsCorrect).Count();
        }
        #endregion



        #region "Select"
        public ExamAnswer GetRecord_ById(int Id)
        {
            return _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswersId == Id).FirstOrDefault();
        }
        public ExamAnswer GetRecord_ByQuestionId_ExamAnswerSetId(int QuestionId, int ExamAnswerSetId)
        {
            return _dbContext.ExamAnswers.AsNoTracking().Where(x => x.QuestionId == QuestionId && x.ExamAnswerSetId == ExamAnswerSetId).FirstOrDefault();
        }
        public IEnumerable<ExamAnswer> GetRecords_ByExamAnswerSetId(int ExamAnswerSetId)
        {
            return _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswerSetId == ExamAnswerSetId);
        }
        public IEnumerable<ExamAnswer> GetReviewQuestion_ByExamAnswerSetId(int ExamAnswerSetId)
        {
            return _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswerSetId == ExamAnswerSetId && x.ReviewQuestion == true).OrderBy(x => x.QuestionRenderOrder);
        }
        public ExamAnswer GetRecord_ByExamAnswerSetId_QuestionId(int ExamAnswerSetId, int QuestionId)
        {
            return _dbContext.ExamAnswers.AsNoTracking().FirstOrDefault(x => x.ExamAnswerSetId == ExamAnswerSetId && x.QuestionId == QuestionId);         
        }
        public ExamAnswer GetRecord_ByExamAnswerSetId_QuestionNo(int ExamAnswerSetId, int QuestionNo)
        {
            var lstExamAnswers = _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswerSetId == ExamAnswerSetId).OrderBy(x => x.QuestionRenderOrder).ToList();
            if (lstExamAnswers.Count < QuestionNo + 1)
            {
                return null;
            }
            else
            {
                return lstExamAnswers[QuestionNo];
            }
        }
        public ExamAnswer GetRecord_ByExamAnswerSetId_QuestionNo_CorrectOnly(int ExamAnswerSetId, int QuestionNo)
        {
            var lstExamAnswers = _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswerSetId == ExamAnswerSetId && x.IsCorrect).OrderBy(x => x.QuestionRenderOrder).ToList();
            if (lstExamAnswers.Count < QuestionNo + 1)
            {
                return null;
            }
            else
            {
                return lstExamAnswers[QuestionNo];
            }
        }
        public ExamAnswer GetRecord_ByExamAnswerSetId_QuestionNo_IncorrectOnly(int ExamAnswerSetId, int QuestionNo)
        {
            var lstExamAnswers = _dbContext.ExamAnswers.AsNoTracking().Where(x => x.ExamAnswerSetId == ExamAnswerSetId && !x.IsCorrect).OrderBy(x => x.QuestionRenderOrder).ToList();
            if (lstExamAnswers.Count < QuestionNo + 1)
            {
                return null;
            }
            else
            {
                return lstExamAnswers[QuestionNo];
            }
        }
        public IEnumerable<ExamAnswer> GetAll()
        {
            return _dbContext.ExamAnswers.AsNoTracking();
        }
        public List<KeyValuePair> GetAll_ExamAnswerSetId_QuestionId()
        {
            //Get only what we need from db as a generic list.
            var lst = _dbContext.ExamAnswers.AsNoTracking()
                .Select(x => new
                {
                    ExamAnswerSetId = x.ExamAnswerSetId,
                    QuestionId = x.QuestionId

                }).ToList();

            //Convert list to class type and return
            return lst.Select(x => new KeyValuePair
            {
                IntKey = x.ExamAnswerSetId,
                IntValue = x.QuestionId

            }).ToList();
        }
        #endregion



        #region "Add"
        public void AddRecord(ExamAnswer Record)
        {
            _dbContext.ExamAnswers.Add(Record);
            Save();
        }

        public void BulkAddRecord(List<ExamAnswer> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        #endregion



        #region "Update"
        public void UpdateRecord(ExamAnswer Record)
        {
            //_dbContext.Entry(Record).State = EntityState.Modified;
            //_dbContext.ExamAnswers(Record).State = EntityState.Modified;
            _dbContext.Set<ExamAnswer>().AddOrUpdate(Record);
            Save();
        }

        public void BulkUpdateRecord(List<ExamAnswer> LstRecords)
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

