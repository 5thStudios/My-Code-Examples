using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IMeasurementTypeRepository
    {
        MeasurementType GetRecord_byId(int RecordId);
        MeasurementType GetRecord_byType(string Type);
        IEnumerable<MeasurementType> GetList();
        //IEnumerable<MeasurementType> GetList_byMeasurementType(Boolean IsMetric = false, Boolean IsFluid = false);


        void AddRecord(MeasurementType Record);
        void UpdateRecord(MeasurementType Record);
        void DeleteRecord(int id);

        void Save();
    }
}

