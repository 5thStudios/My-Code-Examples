using bl.EF;
using System.Collections.Generic;



namespace bl.Repositories
{
    public interface IScrumActivityRepository
    {
        ScrumActivity GetRecord_byId(int Id);
        IEnumerable<ScrumActivity> GetList_byCardId(int CardId);


        void AddRecord(ScrumActivity Record);
        void UpdateRecord(ScrumActivity Record);
        void DeleteRecord(int Id);

        void Save();
    }
}

