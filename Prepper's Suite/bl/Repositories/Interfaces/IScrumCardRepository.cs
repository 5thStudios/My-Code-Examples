using bl.EF;
using System.Collections.Generic;



namespace bl.Repositories
{
    public interface IScrumCardRepository
    {
        ScrumCard GetRecord_byId(int Id);
        IEnumerable<ScrumCard> GetList_byAccountId(int AccountId);


        void AddRecord(ScrumCard Record);
        void UpdateRecord(ScrumCard Record);
        void DeleteRecord(int Id);

        void Save();
    }
}

