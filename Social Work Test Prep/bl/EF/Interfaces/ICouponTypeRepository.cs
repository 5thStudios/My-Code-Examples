using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface ICouponTypeRepository
    {
        CouponType Obtain_byId(int Id);
        CouponType Obtain_byType(string Type);
    }
}