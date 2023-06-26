using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface ICouponRepository
    {
        int GetCount();
        Boolean CodeValid(string Code);
        void IncrementTimesUsed(string Code);

        List<Coupon> GetAll();
        Coupon Obtain_byId(int Id);
        Coupon Obtain_byCode(string Code);

        string ObtainCode_byId(int Id);
        int ObtainId_byCode(string Code);

        void AddRecord(Coupon Record);
        void UpdateRecord(Coupon Record);

        void BulkAddRecord(List<Coupon> LstRecords);
        void BulkUpdateRecord(List<Coupon> LstRecords);
    }
}