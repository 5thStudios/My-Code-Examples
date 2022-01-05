using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IMeasurementStateRepository
    {
        MeasurementState GetRecord_byId(int RecordId);
        int GetId_byState(string state);
        IEnumerable<MeasurementState> GetList();

        void AddRecord(MeasurementState Record);
        void UpdateRecord(MeasurementState Record);
        void DeleteRecord(int id);

        void Save();
    }
}

