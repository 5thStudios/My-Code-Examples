using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using bl.EF;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using N.EntityFramework.Extensions;

namespace Repositories
{
    public class CouponRepository : ICouponRepository
    {

        #region "Properties & Constructors"
        //Properties
        private readonly EF_SwtpDb _dbContext;

        //Constructors
        public CouponRepository()
        {
            _dbContext = new EF_SwtpDb();
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        public CouponRepository(EF_SwtpDb context)
        {
            _dbContext = context;
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
        }
        #endregion



        #region "Methods"
        public int GetCount()
        {
            return _dbContext.Coupons.AsNoTracking().Count();
        }
        public Boolean CodeValid(string Code)
        {
            if (_dbContext.Coupons.AsNoTracking().Where(x => x.Code == Code && x.Enabled).Any())
            {
                //Get coupon
                Coupon _coupon = _dbContext.Coupons.AsNoTracking().FirstOrDefault(x => x.Code == Code && x.Enabled);

                //
                switch (_coupon.CouponTypeId)
                {
                    case 1: //Normal
                        return true;

                    case 2: //Expires
                        if (((DateTime)_coupon.ExpireDate).Date < DateTime.Today.Date)
                        {
                            //expired coupon.  
                            _coupon.Enabled = false;
                            UpdateRecord(_coupon);
                            return false;
                        }
                        else
                            return true;

                    case 3: //Times Used Limit    
                        if (_coupon.TimesUsed >= _coupon.TimesUsedLimit)
                        {
                            //maxed out coupon.  
                            _coupon.Enabled = false;
                            UpdateRecord(_coupon);
                            return false;
                        }
                        else
                            return true;

                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
        public void IncrementTimesUsed(string Code)
        {
            try
            {
                Coupon coupon = _dbContext.Coupons.AsNoTracking().FirstOrDefault(x => x.Code == Code && x.Enabled);
                if (coupon != null)
                {
                    coupon.TimesUsed += 1;
                    UpdateRecord(coupon);
                }
            }
            catch { }
        }
        #endregion



        #region "Select"
        public List<Coupon> GetAll()
        {
            return _dbContext.Coupons.AsNoTracking().ToList();
        }
        public Coupon Obtain_byId(int Id)
        {
            return _dbContext.Coupons.AsNoTracking().Where(x => x.CouponId == Id).FirstOrDefault();
        }
        public Coupon Obtain_byCode(string Code)
        {
            return _dbContext.Coupons.AsNoTracking().Where(x => x.Code == Code).FirstOrDefault();
        }

        public string ObtainCode_byId(int Id)
        {
            return _dbContext.Coupons.AsNoTracking().Where(x => x.CouponId == Id).FirstOrDefault().Code;
        }
        public int ObtainId_byCode(string Code)
        {
            return _dbContext.Coupons.AsNoTracking().Where(x => x.Code == Code).FirstOrDefault().CouponId;
        }
        #endregion



        #region "Add/Update"
        public void AddRecord(Coupon Record)
        {
            _dbContext.Coupons.Add(Record);
            Save();
        }
        public void UpdateRecord(Coupon Record)
        {
            _dbContext.Entry(Record).State = EntityState.Modified;
            Save();
        }

        public void BulkAddRecord(List<Coupon> LstRecords)
        {
            _dbContext.BulkInsert(LstRecords, options => options.BatchSize = 1000);
        }
        public void BulkUpdateRecord(List<Coupon> LstRecords)
        {
            _dbContext.BulkUpdate(LstRecords, options => options.BatchSize = 1000);
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

