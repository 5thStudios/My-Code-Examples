//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v13.5.1+2e068bb
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

namespace www.Models.PublishedModels
{
	/// <summary>Video</summary>
	[PublishedModel("MediaVideo")]
	public partial class MediaVideo : PublishedContentModel
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		public new const string ModelTypeAlias = "MediaVideo";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Media;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<MediaVideo, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public MediaVideo(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		///<summary>
		/// Video Date
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[ImplementPropertyType("MediaVideoDate")]
		public virtual global::System.DateTime MediaVideoDate => this.Value<global::System.DateTime>(_publishedValueFallback, "MediaVideoDate");

		///<summary>
		/// Video Description
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("MediaVideoDescription")]
		public virtual string MediaVideoDescription => this.Value<string>(_publishedValueFallback, "MediaVideoDescription");

		///<summary>
		/// Display Title
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("MediaVideoDisplayTitle")]
		public virtual string MediaVideoDisplayTitle => this.Value<string>(_publishedValueFallback, "MediaVideoDisplayTitle");

		///<summary>
		/// Video Embed Url
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("MediaVideoEmbedUrl")]
		public virtual string MediaVideoEmbedUrl => this.Value<string>(_publishedValueFallback, "MediaVideoEmbedUrl");

		///<summary>
		/// Video Link Url
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("MediaVideoLinkUrl")]
		public virtual string MediaVideoLinkUrl => this.Value<string>(_publishedValueFallback, "MediaVideoLinkUrl");

		///<summary>
		/// Thumbnail
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.5.1+2e068bb")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("MediaVideoThumbnail")]
		public virtual global::Umbraco.Cms.Core.PropertyEditors.ValueConverters.ImageCropperValue MediaVideoThumbnail => this.Value<global::Umbraco.Cms.Core.PropertyEditors.ValueConverters.ImageCropperValue>(_publishedValueFallback, "MediaVideoThumbnail");
	}
}