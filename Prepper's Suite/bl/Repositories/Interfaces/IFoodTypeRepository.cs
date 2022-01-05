using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IFoodTypeRepository
    {
        FoodType GetRecord_byId(int RecordId);
        IEnumerable<FoodType> GetList();

        void AddRecord(FoodType Record);
        void UpdateRecord(FoodType Record);
        void DeleteRecord(int id);

        void Save();
    }
}

