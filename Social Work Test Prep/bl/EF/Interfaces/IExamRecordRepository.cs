using bl.EF;
using System.Collections.Generic;
using System;
using bl.Models;

namespace Repositories
{
    public interface IExamRecordRepository
    {
        int GetCount();
        Boolean DoesRecordExist(ExamRecord examRecord);

        ExamRecord GetRecord_ById(int Id);
        List<ExamRecord> GetRecords_Unsubmitted_ByMemberId(int MemberId);
        List<ExamRecord> GetRecords_Submitted_ByMemberId_ExamModeId(int MemberId, int ExamModeId);
        IEnumerable<ExamRecord> GetAll();
        List<ExamRecord> GetAll_ByMemberId(int MemberId);
        List<KeyValuePair> GetAll_Id_ExamId();
        HashSet<int?> GetAllDistinctIDs();
        HashSet<int> GetAllExamRecordIds_ByMemberId(int MemberId);

        void AddRecord(ExamRecord Record);
        void UpdateRecord(ExamRecord Record);

        void BulkAddRecord(List<ExamRecord> LstRecords);
        void BulkUpdateRecord(List<ExamRecord> LstRecords);
    }
}

