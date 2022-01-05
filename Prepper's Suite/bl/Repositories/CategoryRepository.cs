using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using MvcRepositoryDesignPattern_Demo.Models;
using System.Data.Entity;
using bl.EF;
using System.Web.Mvc;

namespace bl.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public CategoryRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public CategoryRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public Category GetRecord_byId(int Id)
        {
            return _dbContext.Categories.Where(x => x.CategoryId == Id).FirstOrDefault();
        }
        public IEnumerable<Category> GetList()
        {
            return _dbContext.Categories;
        }
        public IEnumerable<Category> GetList_byToolId(int ToolId)
        {
            return _dbContext.Categories.Where(x => x.ToolId == ToolId);
        }
        #endregion



        #region "Add"
        public void AddRecord(Category Record)
        {
            _dbContext.Categories.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(Category Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.Categories.Find(Id);
            if (Record != null) _dbContext.Categories.Remove(Record);
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



        #region "Methods"
        #endregion
    }
}

