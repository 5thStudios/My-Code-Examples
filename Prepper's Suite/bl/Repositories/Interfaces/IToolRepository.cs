using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IToolRepository
    {
        IEnumerable<Tool> GetList();

        int GetCount();
        string GetName(int Id);
    }
}

