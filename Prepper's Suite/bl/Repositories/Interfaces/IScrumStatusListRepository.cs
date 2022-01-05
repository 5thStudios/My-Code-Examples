using bl.EF;
using System.Collections.Generic;



namespace bl.Repositories
{
    public interface IScrumStatusListRepository
    {
        IEnumerable<ScrumStatusList> GetList();
    }
}

