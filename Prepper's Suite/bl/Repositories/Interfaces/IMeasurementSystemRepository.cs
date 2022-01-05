using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IMeasurementSystemRepository
    {
        MeasurementSystem GetRecord_byId(int RecordId);
        IEnumerable<MeasurementSystem> GetList();

        void AddRecord(MeasurementSystem Record);
        void UpdateRecord(MeasurementSystem Record);
        void DeleteRecord(int id);

        void Save();
    }
}

