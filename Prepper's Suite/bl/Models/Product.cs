using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bl.Models
{
    public partial class Product
    {
        public string Text { get; set; }
        public List<object> Parsed { get; set; }
        public List<Hint> Hints { get; set; }
    }

    public partial class Hint
    {
        public Food Food { get; set; }
        public List<Measure> Measures { get; set; }
    }

    public partial class Food
    {
        public string FoodId { get; set; }
        public string Label { get; set; }
        public Nutrients Nutrients { get; set; }
        public string Category { get; set; }
        public string CategoryLabel { get; set; }
        public string FoodContentsLabel { get; set; }
        public Uri Image { get; set; }
        public List<ServingSize> ServingSizes { get; set; }
        public long? ServingsPerContainer { get; set; }
    }

    public partial class Nutrients
    {
        public long? EnercKcal { get; set; }
        public long? Fat { get; set; }
        public double? Chocdf { get; set; }
        public double? Sugar { get; set; }
        public long? Procnt { get; set; }
        public long? Na { get; set; }
    }

    public partial class ServingSize
    {
        public Uri Uri { get; set; }
        public string Label { get; set; }
        public long? Quantity { get; set; }
    }

    public partial class Measure
    {
        public Uri Uri { get; set; }
        public string Label { get; set; }
        public double? Weight { get; set; }
    }
}
