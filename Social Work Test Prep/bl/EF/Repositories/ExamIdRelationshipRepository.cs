using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using N.EntityFramework.Extensions;
using Umbraco.ModelsBuilder;

namespace Repositories
{
    public class ExamIdRelationshipRepository : IExamIdRelationshipRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public ExamIdRelationshipRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public ExamIdRelationshipRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Select"
        public List<ExamIDs_Old_New> SelectAll()
        {
            return _dbContext.ExamIDs_Old_New.AsNoTracking().ToList();
        }
        public List<ExamIDs_Old_New> SelectAll_ExceptText()
        {
            //Get only what we need from db as a generic list.
            var lst = _dbContext.ExamIDs_Old_New.AsNoTracking()
                .Select(x => new
                {
                    ExamId_old = x.ExamId_old,
                    ExamId_new = x.ExamId_new,
                    QuestionId_old = x.QuestionId_old,
                    QuestionId_new = x.QuestionId_new,
                    ContentId_new = x.ContentId_new,
                    ContentId_old = x.ContentId_old

                }).ToList();

            //Convert list to class type and return
            return lst.Select(x => new ExamIDs_Old_New
            {
                ExamId_old = x.ExamId_old,
                ExamId_new = x.ExamId_new,
                QuestionId_old = x.QuestionId_old,
                QuestionId_new = x.QuestionId_new,
                ContentId_new = x.ContentId_new,
                ContentId_old = x.ContentId_old

            }).ToList();



            ////get full list from db
            //var lst = _dbContext.ExamIDs_Old_New.AsNoTracking().ToList();

            ////clear all question texts
            //foreach (var record in lst)
            //{
            //    record.QuestionText_old = string.Empty;
            //}
            //return lst;
        }
        #endregion



        #region "Add/Update"
        public void AddRecord(ExamIDs_Old_New Record)
        {
            _dbContext.ExamIDs_Old_New.Add(Record);
            Save();
        }
        public void UpdateRecord(ExamIDs_Old_New Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        public void BulkAddRecord(List<ExamIDs_Old_New> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkUpdateRecord(List<ExamIDs_Old_New> LstRecords)
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

