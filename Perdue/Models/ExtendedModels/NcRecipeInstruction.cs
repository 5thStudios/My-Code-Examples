using Dragonfly.UmbracoHelpers;
using Umbraco.Cms.Infrastructure.ModelsBuilder;



namespace www.Models.PublishedModels
{
    public partial class NcRecipeInstruction
    {
        public enum InstructionType
        {
            RecipeStep,
            Subheading,
            Note,
            Other,
            Unspecified
        }

        ///<summary>
        /// Text Type enum
        ///</summary>
        public InstructionType Type
        {
            get
            {
                var ttDisplay = this.TextTypeDisplay;

                switch (ttDisplay)
                {
                    case "Recipe Step":
                        return InstructionType.RecipeStep;
                        break;
                    case "Subheading":
                        return InstructionType.Subheading;
                        break;
                    case "Note":
                        return InstructionType.Note;
                        break;
                    case "":
                        return InstructionType.Unspecified;
                        break;
                    default:
                        return InstructionType.Other;
                        break;
                }
            }
        }

        ///<summary>
        /// Text Type Display Text
        ///</summary>
        [ImplementPropertyType("TextType")]
        public string TextTypeDisplay
        {
            get { return this.TextType ?? ""; }
            //get { return this.GetSafeString("TextType"); }
        }

        ////<summary>
        ///// Text Type Prevalue Id
        /////</summary>
        //public object TextTypePrevalueId
        //{
        //    get { return this.GetSafePropertyValue("TextType", ""); }
        //}
    }
}
