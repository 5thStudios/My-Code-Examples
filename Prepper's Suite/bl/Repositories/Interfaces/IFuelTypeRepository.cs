using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IFuelTypeRepository
    {
        FuelType GetRecord_byId(int Id);
        IEnumerable<FuelType> GetList();

        void AddRecord(FuelType Record);
        void UpdateRecord(FuelType Record);
        void DeleteRecord(int id);

        void Save();
    }
}

