//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v13.5.2+3431f76
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace Umbraco.Cms.Web.Common.PublishedModels
{
	/// <summary>Video Panel</summary>
	[PublishedModel("videoPanel")]
	public partial class VideoPanel : PublishedElementModel, ICustomProperties
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		public new const string ModelTypeAlias = "videoPanel";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<VideoPanel, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public VideoPanel(IPublishedElement content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		///<summary>
		/// Content
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("content")]
		public virtual string Content => this.Value<string>(_publishedValueFallback, "content");

		///<summary>
		/// Video Image
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("videoImage")]
		public virtual global::Umbraco.Cms.Core.Models.MediaWithCrops VideoImage => this.Value<global::Umbraco.Cms.Core.Models.MediaWithCrops>(_publishedValueFallback, "videoImage");

		///<summary>
		/// Video Link: *Can enter either youtube or vimeo links.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("videoLink")]
		public virtual global::Umbraco.Cms.Core.Models.Link VideoLink => this.Value<global::Umbraco.Cms.Core.Models.Link>(_publishedValueFallback, "videoLink");

		///<summary>
		/// Container Attributes: *Optional attributes added to pre-defined tags  ex:  data-value="X"
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("containerAttributes")]
		public virtual string ContainerAttributes => global::Umbraco.Cms.Web.Common.PublishedModels.CustomProperties.GetContainerAttributes(this, _publishedValueFallback);

		///<summary>
		/// Container Classes: *Optional classes added to pre-defined tags  ex:  interior-wrap  align-center-middle
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("containerClasses")]
		public virtual string ContainerClasses => global::Umbraco.Cms.Web.Common.PublishedModels.CustomProperties.GetContainerClasses(this, _publishedValueFallback);

		///<summary>
		/// Custom Scripts: *Optional styles and scripts can be added within the outer-most tag.    [example] &lt;style scoped&gt; h1 {font-size:50px; } &lt;/ style&gt;
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("customScripts")]
		public virtual string CustomScripts => global::Umbraco.Cms.Web.Common.PublishedModels.CustomProperties.GetCustomScripts(this, _publishedValueFallback);

		///<summary>
		/// Outer-Most Attributes: *Optional attributes added to the outer-most tag.  ex:  data-value="X"
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("outerMostAttributes")]
		public virtual string OuterMostAttributes => global::Umbraco.Cms.Web.Common.PublishedModels.CustomProperties.GetOuterMostAttributes(this, _publishedValueFallback);

		///<summary>
		/// Outer-Most Class: *Optional classes added to the outer-most tag.   ex:  dark-text light-text  no-angle left-angle left-angle-soft right-angle right-angle-soft
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("outerMostClass")]
		public virtual string OuterMostClass => global::Umbraco.Cms.Web.Common.PublishedModels.CustomProperties.GetOuterMostClass(this, _publishedValueFallback);
	}
}
