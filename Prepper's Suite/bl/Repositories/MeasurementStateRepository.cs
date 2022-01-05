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
    public class MeasurementStateRepository : IMeasurementStateRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public MeasurementStateRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public MeasurementStateRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public MeasurementState GetRecord_byId(int Id)
        {
            return _dbContext.MeasurementStates.Where(x => x.MeasurementStateId == Id).FirstOrDefault();
        }
        public int GetId_byState(string state)
        {
            return _dbContext.MeasurementStates.Where(x => x.Type == state).FirstOrDefault().MeasurementStateId;
        }
        public IEnumerable<MeasurementState> GetList()
        {
            return _dbContext.MeasurementStates;
        }
        #endregion



        #region "Add"
        public void AddRecord(MeasurementState Record)
        {
            _dbContext.MeasurementStates.Add(Record);
            Save();
        }
        #endregion



        #region "Update"
        public void UpdateRecord(MeasurementState Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }
        #endregion



        #region "Delete"
        public void DeleteRecord(int Id)
        {
            var Record = _dbContext.MeasurementSystems.Find(Id);
            if (Record != null) _dbContext.MeasurementSystems.Remove(Record);
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

