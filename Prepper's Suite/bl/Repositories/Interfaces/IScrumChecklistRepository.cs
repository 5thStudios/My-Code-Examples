using bl.EF;
using System.Collections.Generic;



namespace bl.Repositories
{
    public interface IScrumChecklistRepository
    {
        ScrumChecklist GetRecord_byId(int Id);
        IEnumerable<ScrumChecklist> GetList_byCardId(int CardId);


        void AddRecord(ScrumChecklist Record);
        void UpdateRecord(ScrumChecklist Record);
        void DeleteRecord(int Id);

        void Save();
    }
}

