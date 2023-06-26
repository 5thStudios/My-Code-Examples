using Newtonsoft.Json;

namespace bl.Models
{
    public partial class ImportMember
    {
        [JsonProperty("User")]
        public string User { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Extend Days by")]
        public string ExtendDaysBy { get; set; }

        [JsonProperty("Exam Name")]
        public string ExamName { get; set; }


        public ImportMember() { }
    }
}
