using bl.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bl.Repositories
{
    public interface IGenderRepository
    {
        List<SelectListItem> GetSelectItemList();
        IEnumerable<Gender> GetList();
    }
}
