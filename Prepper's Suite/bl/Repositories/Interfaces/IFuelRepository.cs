using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IFuelRepository
    {
        Fuel GetRecord_byId(int RecordId);

        void AddRecord(Fuel Record);
        void UpdateRecord(Fuel Record);
        void DeleteRecord(int id);

        void Save();
    }
}

