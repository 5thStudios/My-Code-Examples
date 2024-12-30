namespace www.Models
{
    public class FoodProductTypes
    {
        public List<FoodProductType> Types { get; set; }
        public List<string> Errors { get; set; }

        public FoodProductTypes()
        {
            Types = new List<FoodProductType>();
            Errors = new List<string>();
        }
    }


    public class FoodProductType
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public List<FoodProductType> SubTypes { get; set; }

        public FoodProductType()
        {
            SubTypes = new List<FoodProductType>();
        }
    }
}

