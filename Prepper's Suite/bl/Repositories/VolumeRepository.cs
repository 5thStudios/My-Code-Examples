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
    public class VolumeRepository : IVolumeRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public VolumeRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public VolumeRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public Volume GetRecord_byId(int Id)
        {
            return _dbContext.Volumes.Where(x => x.VolumeId == Id).FirstOrDefault();
        }
        public IEnumerable<Volume> GetList()
        {
            return _dbContext.Volumes;
        }
        public IEnumerable<Volume> GetList_byMeasurementTypeId(int MeasurementTypeId)
        {
            return _dbContext.Volumes.Where(x => x.MeasurementTypeId == MeasurementTypeId);
        }
        #endregion



        #region "Add"
        public void AddRecord(Volume Record)
        {
            _dbContext.Volumes.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(Volume Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.Volumes.Find(Id);
            if (Record != null) _dbContext.Volumes.Remove(Record);
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

