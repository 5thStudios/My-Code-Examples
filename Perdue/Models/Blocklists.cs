using System.Text.Json.Serialization;
using Umbraco.Cms.Infrastructure.PublishedCache.DataSource;

/*
 *  Creating Blocklist Programmatically
 *  https://docs.umbraco.com/umbraco-cms/fundamentals/backoffice/property-editors/built-in-umbraco-property-editors/block-editor/block-list-editor#creating-blocklist-programmatically
 */

namespace www.Models.Blocklist
{
    //BLOCKLIST- INSTRUCTIONS
    public class Instruction
    {
        [JsonPropertyName("layout")]
        public Layout layout { get; set; } = new Layout();

        [JsonPropertyName("contentData")]
        public List<Instruction_ContentData> contentData { get; set; } = new List<Instruction_ContentData>();

        [JsonPropertyName("settingsData")]
        public List<object> settingsData { get; set; } = new List<object>();
    }
    public class Instruction_ContentData
    {
        [JsonPropertyName("contentTypeKey")]
        public string contentTypeKey { get; set; }

        [JsonPropertyName("udi")]
        public string udi { get; set; }

        [JsonPropertyName("InstructionText")]
        public string InstructionText { get; set; }

        [JsonPropertyName("TextType")]
        public List<string> TextType { get; set; } = new List<string>();

        [JsonPropertyName("ImportedRecipeId")]
        public string ImportedRecipeId { get; set; } = "";

        [JsonPropertyName("ImportedInstructionId")]
        public string ImportedInstructionId { get; set; } = "";

        [JsonPropertyName("ImportedSequence")]
        public string ImportedSequence { get; set; } = "";
    }






    //=====================================================
    //BLOCKLIST- INGREDIENTS
    public class Ingredient
    {
        [JsonPropertyName("layout")]
        public Layout layout { get; set; } = new Layout();

        [JsonPropertyName("contentData")]
        public List<Ingredient_ContentData> contentData { get; set; } = new List<Ingredient_ContentData>();

        [JsonPropertyName("settingsData")]
        public List<object> settingsData { get; set; } = new List<object>();
    }
    public class Ingredient_ContentData
    {
        [JsonPropertyName("contentTypeKey")]
        public string contentTypeKey { get; set; }

        [JsonPropertyName("udi")]
        public string udi { get; set; }

        [JsonPropertyName("DisplayQuantity")]
        public string DisplayQuantity { get; set; }

        [JsonPropertyName("AbsoluteQty")]
        public string AbsoluteQty { get; set; }

        [JsonPropertyName("QuantityQualifier")]
        public string QuantityQualifier { get; set; }

        [JsonPropertyName("IngredientUnit")]
        public string IngredientUnit { get; set; }

        [JsonPropertyName("Ingredient")]
        public string Ingredient { get; set; }

        [JsonPropertyName("IngredientPrep")]
        public string IngredientPrep { get; set; }

        [JsonPropertyName("IsOptional")]
        public string IsOptional { get; set; }

        [JsonPropertyName("MeasurementNote")]
        public string MeasurementNote { get; set; }

        [JsonPropertyName("MeasurementOz")]
        public string MeasurementOz { get; set; }

        [JsonPropertyName("SubheadNote")]
        public string SubheadNote { get; set; }

        [JsonPropertyName("ImportedUnitName")]
        public string ImportedUnitName { get; set; } = "";

        [JsonPropertyName("ImportedSequenceNumber")]
        public string ImportedSequenceNumber { get; set; } = "";

        [JsonPropertyName("ImportedStepNumber")]
        public string ImportedStepNumber { get; set; } = "";

        [JsonPropertyName("ImportedIngredientId")]
        public string ImportedIngredientId { get; set; } = "";

        [JsonPropertyName("ImportedRecipeId")]
        public string ImportedRecipeId { get; set; } = "";
    }





    //=====================================================
    //GLOBAL CLASSES FOR BLOCKLISTS
    public class Layout
    {
        [JsonPropertyName("Umbraco.BlockList")]
        public List<BlockListUdi> UmbracoBlockList { get; set; } = new List<BlockListUdi>();

    }


    public class BlockListUdi
    {
        [JsonPropertyName("contentUdi")]
        public string contentUdi { get; set; }

    }

}
