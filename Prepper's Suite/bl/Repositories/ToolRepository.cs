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
    public class ToolRepository : IToolRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public ToolRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public ToolRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion




        #region "Select"
        public IEnumerable<Tool> GetList()
        {
            return _dbContext.Tools;
        }
        #endregion



        #region "Add"
        #endregion



        #region "Update"
        #endregion



        #region "Delete"
        #endregion



        #region "Save & Dispose"     
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
        public int GetCount()
        {
            return _dbContext.Tools.Count();
        }
        public string GetName(int Id)
        {
            return _dbContext.Tools.Where(x => x.ToolId == Id).FirstOrDefault().Name;
        }
        #endregion

    }
}

