using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace www.Models.LiveSkuModel
{
    public class LiveSku
    {
        public int Id { get; set; }
        //public string ApiReferenceId { get; set; }
        public string Attribute { get; set; }
        public string AverageCaseCount { get; set; }
        public string AverageSize { get; set; }
        public string AverageWeight { get; set; }
        public string BrandName { get; set; }
        public string Form { get; set; } = string.Empty;
        public string GroupName { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ServingSize { get; set; }
        public string Url { get; set; }
        public string ProductType { get; set; }
        public string ProductSubtype { get; set; }
        public List<string> LstCVPSubtypes { get; set; }


        public LiveSku()
        {
            Attribute = "Fresh";
            LstCVPSubtypes = new List<string>();
        }
    }
}
