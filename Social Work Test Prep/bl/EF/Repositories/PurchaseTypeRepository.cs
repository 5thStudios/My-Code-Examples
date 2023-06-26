using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using bl.EF;


namespace Repositories
{
    public class PurchaseTypeRepository : IPurchaseTypeRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public PurchaseTypeRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public PurchaseTypeRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Select"
        public string ObtainType_byId(int Id)
        {
            return _dbContext.PurchaseTypes.AsNoTracking().Where(x => x.PurchaseTypeId == Id).FirstOrDefault().Type;
        }
        public int ObtainId_byType(string Type)
        {
            return _dbContext.PurchaseTypes.AsNoTracking().Where(x => x.Type == Type).FirstOrDefault().PurchaseTypeId;
        }
        public IEnumerable<PurchaseType> SelectAll()
        {
            return _dbContext.PurchaseTypes.AsNoTracking();
        }
        #endregion

    }
}

