using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using bl.EF;


namespace bl.Repositories
{
    public class ScrumStatusListRepository : IScrumStatusListRepository
    {
        #region "Properties & Constructors"
        //Properties
        private readonly EFPrepperSuiteDb _dbContext;

        //Constructors
        public ScrumStatusListRepository()
        {
            _dbContext = new EFPrepperSuiteDb();
        }
        public ScrumStatusListRepository(EFPrepperSuiteDb context)
        {
            _dbContext = context;
        }
        #endregion



        #region "Select"
        public IEnumerable<ScrumStatusList> GetList()
        {
            return _dbContext.ScrumStatusLists;
        }
        #endregion


    }
}

