using bl.EF;
using System;
using System.Collections.Generic;


namespace Repositories
{
    public interface IExamIdRelationshipRepository
    {
        List<ExamIDs_Old_New> SelectAll();
        List<ExamIDs_Old_New> SelectAll_ExceptText();

        void AddRecord(ExamIDs_Old_New Record);
        void UpdateRecord(ExamIDs_Old_New Record);
        void BulkAddRecord(List<ExamIDs_Old_New> LstRecords);
        void BulkUpdateRecord(List<ExamIDs_Old_New> LstRecords);
    }
}

