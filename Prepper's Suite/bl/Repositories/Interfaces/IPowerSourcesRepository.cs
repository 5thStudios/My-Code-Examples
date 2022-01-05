using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Repositories
{
    public interface IPowerSourcesRepository
    {
        PowerSource GetRecord_byId(int Id);

        void AddRecord(PowerSource Record);
        void UpdateRecord(PowerSource Record);
        void DeleteRecord(int Id);


        void Save();
    }
}