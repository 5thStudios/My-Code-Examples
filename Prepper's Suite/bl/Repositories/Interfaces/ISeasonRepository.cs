using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface ISeasonRepository
    {
        Season GetRecord_byId(int RecordId);
        IEnumerable<Season> GetList();

        void AddRecord(Season Record);
        void UpdateRecord(Season Record);
        void DeleteRecord(int id);

        void Save();
    }
}

