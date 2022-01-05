using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IVolumeRepository
    {
        Volume GetRecord_byId(int RecordId);
        IEnumerable<Volume> GetList();
        IEnumerable<Volume> GetList_byMeasurementTypeId(int MeasurementTypeId);

        void AddRecord(Volume Record);
        void UpdateRecord(Volume Record);
        void DeleteRecord(int id);

        void Save();
    }
}

