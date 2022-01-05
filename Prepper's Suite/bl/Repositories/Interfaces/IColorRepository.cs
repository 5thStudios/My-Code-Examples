using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IColorRepository
    {
        Color GetRecord_byId(int Id);
        IEnumerable<Color> GetList();
    }
}
