using bl.EF;
using System.Collections.Generic;


namespace Repositories
{
    public interface IExamModeRepository
    {
        int GetCount();
        List<ExamMode> SelectAll();
        int GetIdByMode(string Mode);
        string GetModeById(int Id);
        string GetModeNameById(int Id);
    }
}

