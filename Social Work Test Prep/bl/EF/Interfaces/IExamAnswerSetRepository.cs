using bl.EF;
using System.Collections.Generic;


namespace Repositories
{
    public interface IExamAnswerSetRepository
    {
        int GetCount();
        ExamAnswerSet GetRecord_ById(int Id);
        ExamAnswerSet GetRecord_ByExamRecordId(int Id);
        IEnumerable<ExamAnswerSet> GetAll();

        void AddRecord(ExamAnswerSet Record);
        void UpdateRecord(ExamAnswerSet Record);
    }
}

