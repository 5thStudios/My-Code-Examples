using System;
using System.Collections.Generic;
using System.Linq;
using bl.EF;


namespace Repositories
{
    public class ExamModeRepository : IExamModeRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public ExamModeRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public ExamModeRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Methods"
        public int GetCount()
        {
            return _dbContext.ExamModes.AsNoTracking().Count();
        }
        #endregion



        #region "Select"
        public List<ExamMode> SelectAll()
        {
            return _dbContext.ExamModes.AsNoTracking().ToList();
        }

        public int GetIdByMode(string Mode)
        {
            return _dbContext.ExamModes.AsNoTracking().Where(x => x.Mode == Mode).FirstOrDefault().ExamModeId;
        }
        public string GetModeById(int Id)
        {
            return _dbContext.ExamModes.AsNoTracking().Where(x => x.ExamModeId == Id).FirstOrDefault().Mode;
        }
        public string GetModeNameById(int Id)
        {
            return _dbContext.ExamModes.AsNoTracking().Where(x => x.ExamModeId == Id).FirstOrDefault().Name;
        }
        #endregion
    }
}

