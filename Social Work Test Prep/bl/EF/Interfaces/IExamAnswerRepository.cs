using bl.EF;
using bl.Models;
using System.Collections.Generic;


namespace Repositories
{
    public interface IExamAnswerRepository
    {
        int GetCount();

        int GetCount_ByExamAnswerSetId(int ExamAnswerSetId);
        int GetCount_ByExamAnswerSetId_CorrectAnswers(int ExamAnswerSetId);
        int GetCount_ByExamAnswerSetId_IncorrectAnswers(int ExamAnswerSetId);

        int GetCount_ByExamAnswerSetId_ContentAreaId(int ExamAnswerSetId, int ContentAreaId);
        int GetCount_ByExamAnswerSetId_ContentAreaId_CorrectAnswer(int ExamAnswerSetId, int ContentAreaId);

        IEnumerable<ExamAnswer> GetAll();
        List<KeyValuePair> GetAll_ExamAnswerSetId_QuestionId();


        ExamAnswer GetRecord_ById(int Id);
        ExamAnswer GetRecord_ByQuestionId_ExamAnswerSetId(int QuestionId, int ExamAnswerSetId);
        IEnumerable<ExamAnswer> GetRecords_ByExamAnswerSetId(int ExamAnswerSetId);
        IEnumerable<ExamAnswer> GetReviewQuestion_ByExamAnswerSetId(int ExamAnswerSetId);
        ExamAnswer GetRecord_ByExamAnswerSetId_QuestionId(int ExamAnswerSetId, int QuestionId);
        ExamAnswer GetRecord_ByExamAnswerSetId_QuestionNo(int ExamAnswerSetId, int QuestionNo);
        ExamAnswer GetRecord_ByExamAnswerSetId_QuestionNo_CorrectOnly(int ExamAnswerSetId, int QuestionNo);
        ExamAnswer GetRecord_ByExamAnswerSetId_QuestionNo_IncorrectOnly(int ExamAnswerSetId, int QuestionNo);

        void AddRecord(ExamAnswer Record);
        void UpdateRecord(ExamAnswer Record);

        void BulkAddRecord(List<ExamAnswer> LstRecords);
        void BulkUpdateRecord(List<ExamAnswer> LstRecords);
    }
}

