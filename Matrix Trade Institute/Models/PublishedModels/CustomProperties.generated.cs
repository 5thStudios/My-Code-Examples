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
	// Mixin Content Type with alias "customProperties"
	/// <summary>Custom Properties</summary>
	public partial interface ICustomProperties : IPublishedElement
	{
		/// <summary>Container Attributes</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		string ContainerAttributes { get; }

		/// <summary>Container Classes</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		string ContainerClasses { get; }

		/// <summary>Custom Scripts</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		string CustomScripts { get; }

		/// <summary>Outer-Most Attributes</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		string OuterMostAttributes { get; }

		/// <summary>Outer-Most Class</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		string OuterMostClass { get; }
	}

	/// <summary>Custom Properties</summary>
	[PublishedModel("customProperties")]
	public partial class CustomProperties : PublishedElementModel, ICustomProperties
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		public new const string ModelTypeAlias = "customProperties";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<CustomProperties, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public CustomProperties(IPublishedElement content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		///<summary>
		/// Container Attributes: *Optional attributes added to pre-defined tags  ex:  data-value="X"
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("containerAttributes")]
		public virtual string ContainerAttributes => GetContainerAttributes(this, _publishedValueFallback);

		/// <summary>Static getter for Container Attributes</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static string GetContainerAttributes(ICustomProperties that, IPublishedValueFallback publishedValueFallback) => that.Value<string>(publishedValueFallback, "containerAttributes");

		///<summary>
		/// Container Classes: *Optional classes added to pre-defined tags  ex:  interior-wrap  align-center-middle
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("containerClasses")]
		public virtual string ContainerClasses => GetContainerClasses(this, _publishedValueFallback);

		/// <summary>Static getter for Container Classes</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static string GetContainerClasses(ICustomProperties that, IPublishedValueFallback publishedValueFallback) => that.Value<string>(publishedValueFallback, "containerClasses");

		///<summary>
		/// Custom Scripts: *Optional styles and scripts can be added within the outer-most tag.    [example] &lt;style scoped&gt; h1 {font-size:50px; } &lt;/ style&gt;
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("customScripts")]
		public virtual string CustomScripts => GetCustomScripts(this, _publishedValueFallback);

		/// <summary>Static getter for Custom Scripts</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static string GetCustomScripts(ICustomProperties that, IPublishedValueFallback publishedValueFallback) => that.Value<string>(publishedValueFallback, "customScripts");

		///<summary>
		/// Outer-Most Attributes: *Optional attributes added to the outer-most tag.  ex:  data-value="X"
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("outerMostAttributes")]
		public virtual string OuterMostAttributes => GetOuterMostAttributes(this, _publishedValueFallback);

		/// <summary>Static getter for Outer-Most Attributes</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static string GetOuterMostAttributes(ICustomProperties that, IPublishedValueFallback publishedValueFallback) => that.Value<string>(publishedValueFallback, "outerMostAttributes");

		///<summary>
		/// Outer-Most Class: *Optional classes added to the outer-most tag.   ex:  dark-text light-text  no-angle left-angle left-angle-soft right-angle right-angle-soft
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("outerMostClass")]
		public virtual string OuterMostClass => GetOuterMostClass(this, _publishedValueFallback);

		/// <summary>Static getter for Outer-Most Class</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.2+3431f76")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static string GetOuterMostClass(ICustomProperties that, IPublishedValueFallback publishedValueFallback) => that.Value<string>(publishedValueFallback, "outerMostClass");
	}
}