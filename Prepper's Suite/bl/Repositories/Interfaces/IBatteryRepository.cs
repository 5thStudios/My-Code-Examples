using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IBatteryRepository
    {
        Battery GetRecord_byId(int Id);

        void AddRecord(Battery Record);
        void UpdateRecord(Battery Record);
        void DeleteRecord(int id);

        void Save();
    }
}

