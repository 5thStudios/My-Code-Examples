using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface ICategoryRepository
    {
        Category GetRecord_byId(int Id);
        IEnumerable<Category> GetList();
        IEnumerable<Category> GetList_byToolId(int ToolId);

        void AddRecord(Category Record);
        void UpdateRecord(Category Record);
        void DeleteRecord(int id);

        void Save();
    }
}

