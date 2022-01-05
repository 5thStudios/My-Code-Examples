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
    public class MeasurementTypeRepository : IMeasurementTypeRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public MeasurementTypeRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public MeasurementTypeRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public MeasurementType GetRecord_byId(int Id)
        {
            return _dbContext.MeasurementTypes.Where(x => x.MeasurementTypeId == Id).FirstOrDefault();
        }
        public MeasurementType GetRecord_byType(string Type)
        {
            return _dbContext.MeasurementTypes.Where(x => x.Type == Type).FirstOrDefault();
        }
        public IEnumerable<MeasurementType> GetList()
        {
            return _dbContext.MeasurementTypes;
        }
        //public IEnumerable<MeasurementType> GetList_byMeasurementType(Boolean IsMetric = false, Boolean IsFluid = false)
        //{
        //    return _dbContext.MeasurementTypes.Where(x => x.IsMetric == IsMetric && x.IsFluid == IsFluid);
        //}
        #endregion



        #region "Add"
        public void AddRecord(MeasurementType Record)
        {
            _dbContext.MeasurementTypes.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(MeasurementType Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.MeasurementTypes.Find(Id);
            if (Record != null) _dbContext.MeasurementTypes.Remove(Record);
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

