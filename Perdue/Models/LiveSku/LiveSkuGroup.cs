using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace www.Models.LiveSkuModel
{
    public class LiveSkuGroup
    {
        public string GroupName { get; set; }
        public List<LiveSku> LstLiveSkus { get; set; }


        public LiveSkuGroup()
        {
            LstLiveSkus = new List<LiveSku>();
        }
    }
}
