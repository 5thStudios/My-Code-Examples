using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IBatteryTypeRepository
    {
        BatteryType GetRecord_byId(int Id);
        IEnumerable<BatteryType> GetList();

        void AddRecord(BatteryType Record);
        void UpdateRecord(BatteryType Record);
        void DeleteRecord(int id);

        void Save();
    }
}

