using System;
using System.Collections.Generic;
using System.Linq;
using bl.EF;


namespace Repositories
{
    public class CouponTypeRepository : ICouponTypeRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public CouponTypeRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public CouponTypeRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Select"
        public CouponType Obtain_byId(int Id)
        {
            return _dbContext.CouponTypes.AsNoTracking().Where(x => x.CouponTypeId == Id).FirstOrDefault();
        }
        public CouponType Obtain_byType(string Type)
        {
            return _dbContext.CouponTypes.AsNoTracking().Where(x => x.Type == Type).FirstOrDefault();
        }
        #endregion
    }
}

