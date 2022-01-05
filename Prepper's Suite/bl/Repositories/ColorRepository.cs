using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using bl.EF;
using System.Web.Mvc;

namespace bl.Repositories
{
    public class ColorRepository : IColorRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public ColorRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public ColorRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Methods"
        #endregion



        #region "Select"
        public IEnumerable<Color> GetList()
        {
            return _dbContext.Colors;
        }
        public Color GetRecord_byId(int Id)
        {
            return _dbContext.Colors.Where(x => x.ColorId == Id).FirstOrDefault();
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
