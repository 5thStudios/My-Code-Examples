using System.Web;
using Umbraco.Cms.Core.Models;
using www.Models;
using www.Models.PublishedModels;

namespace www.ViewModels
{
    public class ProductServing
    {
        public Servings Servings { get; set; } = new Servings();
        public Calories Calories { get; set; } = new Calories();
        public Fat Fat { get; set; } = new Fat();
        public Cholesterol Cholesterol { get; set; } = new Cholesterol();
        public Sodium Sodium { get; set; } = new Sodium();
        public Carbohydrates Carbohydrates { get; set; } = new Carbohydrates();
        public Protein Protein { get; set; } = new Protein();
        public VitaminsMinerals VitaminsMinerals { get; set; } = new VitaminsMinerals();
    }

    public class Servings
    {
        public string? servingSize { get; set; }
        public string? servingSizeDescription { get; set; }
        public string? servingsPerCase { get; set; }
    }

    public class Calories
    {
        public string? calories { get; set; }
        public string? caloriesFromFat { get; set; }
    }

    public class Fat
    {
        public string? totalFat { get; set; }
        public string? totalFatPercent { get; set; }
        public string? saturatedFat { get; set; }
        public string? saturatedFatPercent { get; set; }
        public string? transFat { get; set; }
    }

    public class Cholesterol
    {
        public string? cholesterol { get; set; }
        public string? cholesterolPercent { get; set; }
    }

    public class Sodium
    {
        public string? sodium { get; set; }
        public string? sodiumPercent { get; set; }
    }

    public class Carbohydrates
    {
        public string? totalCarbohydrates { get; set; }
        public string? totalCarbohydratesPercent { get; set; }
        public string? dietaryFiber { get; set; }
        public string? dietaryFiberPercent { get; set; }
        public string? sugars { get; set; }
        public string? addedSugar { get; set; }
        public string? addedSugarPercent { get; set; }
    }

    public class Protein
    {
        public string? protein { get; set; }
        public string? proteinPercent { get; set; }
    }

    public class VitaminsMinerals
    {
        public string? vitaminAPercent { get; set; }
        public string? vitaminCPercent { get; set; }
        public string? vitaminD { get; set; }
        public string? vitaminDPercent { get; set; }
        public string? calcium { get; set; }
        public string? calciumPercent { get; set; }
        public string? iron { get; set; }
        public string? ironPercent { get; set; }
        public string? potassium { get; set; }
        public string? potassiumPercent { get; set; }
    }
}

