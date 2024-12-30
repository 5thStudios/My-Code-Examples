using Dragonfly.UmbracoHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Extensions;
using System.Linq.Expressions;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core;



namespace www.Models.PublishedModels
{
    public partial class NcRecipeIngredient //: PublishedElementModel
    {



        //[ImplementPropertyType("IngredientUnit")]
        //public virtual global::Umbraco.Cms.Core.Models.PublishedContent.IPublishedContent IngredientUnit => this.Value<global::Umbraco.Cms.Core.Models.PublishedContent.IPublishedContent>(_publishedValueFallback, "IngredientUnit");


        ///<summary>
        /// Unit
        ///</summary>
        ///
        //[ImplementPropertyType("IngredientUnit")]
        //public IngredientUnit xIngredientUnit
        //{
        //    get
        //    {
        //        //var value = this.IngredientUnit.GetSafePropertyValue<IEnumerable<IPublishedContent>>("IngredientUnit");
        //        var value = this.IngredientUnit;

        //        //var value = this.GetSafePropertyValue<IEnumerable<IPublishedContent>>("IngredientUnit");
        //        if (value != null)
        //        {
        //            //return value.FirstOrDefault() as IngredientUnit;
        //            return value as IngredientUnit;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}



        public enum ItemType
        {
            Ingredient,
            Subhead,
            Unknown
        }

        public ItemType Type
        {
            get
            {
                if (this.SubheadNote != null && this.SubheadNote != "")
                {
                    return ItemType.Subhead;
                }
                else if (this.Ingredient != null && this.Ingredient != "")
                {
                    return ItemType.Ingredient;
                }
                else
                {
                    return ItemType.Unknown;
                }
            }
        }



        ///<summary>
        /// Sequence Number
        ///</summary>
        public int ImportedIngredientSequenceNumber
        {
            get
            {
                //var propVal = this.GetSafePropertyValue<string>("ImportedSequenceNumber");
                var propVal = this.ImportedSequenceNumber;
                if (string.IsNullOrEmpty(propVal))
                {
                    return 0;
                }
                else
                {
                    var number = 0;
                    var isNumber = Int32.TryParse(propVal, out number);
                    if (isNumber)
                    {
                        return number;
                    }
                    else
                    {
                        return 0;
                    }
                }

            }
            }
        }
}
