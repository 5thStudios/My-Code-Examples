using bl.EF;
using System.Collections.Generic;
using System;

namespace Repositories
{
    public interface IPurchaseTypeRepository
    {
        string ObtainType_byId(int Id);
        int ObtainId_byType(string Type);
        IEnumerable<PurchaseType> SelectAll();
    }
}