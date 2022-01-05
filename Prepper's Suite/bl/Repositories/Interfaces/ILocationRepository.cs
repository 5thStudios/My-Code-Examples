using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface ILocationRepository
    {
        Location GetRecord_byId(int Id);
        IEnumerable<Location> GetList_byAccountId(int AccountId);
        IEnumerable<Location> GetBugoutBagList_byAccountId(int AccountId);

        void AddRecord(Location Record);
        void UpdateRecord(Location Record);
        void DeleteRecord(int id);


        void Save();
    }
}

