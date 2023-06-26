//https://www.justnik.me/blog/indexing-sort-able-dates-in-umbraco-version-8

using Examine;
using System;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Examine.Providers;
using Umbraco.Web.Runtime;

namespace ExamineIndexing.ExamineHelper
{
    public class IndexerComponent : IComponent
    {
        private readonly IExamineManager examineManager;
        private readonly IUmbracoContextFactory umbracoContextFactory;
        private readonly ILogger logger;

        public IndexerComponent(IExamineManager examineManager,
            IUmbracoContextFactory umbracoContextFactory,
            ILogger logger)
        {
            this.examineManager = examineManager ?? throw new ArgumentNullException(nameof(examineManager));
            this.umbracoContextFactory = umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize()
        {
            if (examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex externalIndex))
            {
                //Add new field to the definition collection for date sorting
                externalIndex.FieldDefinitionCollection.AddOrUpdate(new FieldDefinition("postDateSortable", FieldDefinitionTypes.Long));

                //
                ((BaseIndexProvider)externalIndex).TransformingIndexValues += IndexerComponent_TransformingIndexValues;
            }
        }

        private void IndexerComponent_TransformingIndexValues(object sender, IndexingItemEventArgs e)
        {
            try
            {
                if (int.TryParse(e.ValueSet.Id, out var nodeId))
                    switch (e.ValueSet.ItemType)
                    {
                        case "post":
                            using (var umbracoContext = umbracoContextFactory.EnsureUmbracoContext())
                            {
                                var contentNode = umbracoContext.UmbracoContext.Content.GetById(nodeId);
                                if (contentNode != null)
                                {
                                    var articleDate = contentNode.Value<DateTime>("postDate");
                                    e.ValueSet.Set("postDateSortable", articleDate.Date.Ticks);
                                }
                            }
                            break;
                    }
            }
            catch (Exception ex)
            {
                logger.Error<IndexerComponent>(ex);
            }

        }

        public void Terminate() { }
    }

    //Tells Umbraco to register the component above in the application
    [ComposeAfter(typeof(WebFinalComposer))]
    public class RegisterIndexerComponentComposer : IComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<IndexerComponent>();
        }
    }

}