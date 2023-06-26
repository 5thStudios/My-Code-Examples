using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using bl.Models;
using N.EntityFramework.Extensions;
using Umbraco.ModelsBuilder;

namespace Repositories
{
    public class OriginalUmbracoNodeRepository : IOriginalUmbracoNodeRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public OriginalUmbracoNodeRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public OriginalUmbracoNodeRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Select"
        public HashSet<int> GetAllIDs()
        {
            return _dbContext.Original_UmbracoNode.AsNoTracking().Select(x => x.id).ToHashSet();
        }
        public IEnumerable<Original_UmbracoNode> SelectAll_Lvl4()
        {
            return _dbContext.Original_UmbracoNode.AsNoTracking().Where(x => x.level == 4 && !x.isAddedToSite);
        }
        public IEnumerable<Original_UmbracoNode> SelectAll_Lvl4_ByEmail(string email)
        {
            //return _dbContext.Original_UmbracoNode.AsNoTracking().Where(x => x.level == 4 && x.text == "Ragillespie.1996@gmail.com - 15433433");// && !x.isAddedToSite);
            return _dbContext.Original_UmbracoNode.AsNoTracking().Where(x => x.level == 4 && x.text.Contains(email) && !x.isAddedToSite);
        }
        public IEnumerable<Original_UmbracoNode> SelectAll_Lvl5()
        {
            return _dbContext.Original_UmbracoNode.AsNoTracking().Where(x => x.level == 5 && !x.isAddedToSite);
        }
        public IEnumerable<Original_UmbracoNode> SelectAll_Lvl6()
        {
            return _dbContext.Original_UmbracoNode.AsNoTracking().Where(x => x.level == 6 && !x.isAddedToSite);
        }
        public IEnumerable<Original_UmbracoNode> SelectAll_Lvl7()
        {
            return _dbContext.Original_UmbracoNode.AsNoTracking().Where(x => x.level == 7 && !x.isAddedToSite);
        }
        #endregion



        #region "Add/Update"
        public void AddRecord(Original_UmbracoNode Record)
        {
            _dbContext.Original_UmbracoNode.Add(Record);
            Save();
        }
        public void UpdateRecord(Original_UmbracoNode Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }


        public void BulkAddRecord(List<Original_UmbracoNode> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkUpdateRecord(List<Original_UmbracoNode> LstRecords)
        {
            _dbContext.BulkUpdate(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkDeleteRecords(List<Original_UmbracoNode> LstRecords)
        {
            _dbContext.BulkDelete(LstRecords);
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

    }
}

