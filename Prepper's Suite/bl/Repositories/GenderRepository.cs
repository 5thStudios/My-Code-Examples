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
    public class GenderRepository : IGenderRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public GenderRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public GenderRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Methods"
        #endregion



        #region "Select"
        public IEnumerable<Gender> GetList()
        {
            return _dbContext.Genders;
        }
        public List<SelectListItem> GetSelectItemList()
        {
            return _dbContext.Genders.Select(x => new SelectListItem
            {
                Value = x.GenderId.ToString(),
                Text = x.Name
            }).ToList();
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


    }
}