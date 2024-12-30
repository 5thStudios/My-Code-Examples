using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace RoutingDocs.ContentFinders
{
    public class ECMC404ContentFinder : IContentLastChanceFinder
    {
        private readonly IDomainService _domainService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public ECMC404ContentFinder(
            IDomainService domainService, 
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _domainService = domainService;
            _umbracoContextAccessor = umbracoContextAccessor;
        }




        public Task<bool> TryFindContent(IPublishedRequestBuilder contentRequest)
        {

            // Find the root node with a matching domain to the incoming request
            var allDomains = _domainService.GetAll(true).ToList();
            var domain = allDomains?
                .FirstOrDefault(f => f.DomainName == contentRequest.Uri.Authority
                                     || f.DomainName == $"https://{contentRequest.Uri.Authority}"
                                     || f.DomainName == $"http://{contentRequest.Uri.Authority}");

            var siteId = domain != null ? domain.RootContentId : allDomains.Any() ? allDomains.FirstOrDefault()?.RootContentId : null;

            string? domainName = domain?.DomainName;

            if (!_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
                return Task.FromResult(false);
            }

            if (umbracoContext.Content == null)
                return new Task<bool>(() => contentRequest.PublishedContent is not null);

            var siteRoot = umbracoContext.Content.GetById(false, siteId ?? -1);

            if (siteRoot is null)
            {
                return Task.FromResult(false);
            }



            // Use the document id of the 404 page
            var notFoundNode = siteRoot.Children?.FirstOrDefault(f => f.Name == "Our Website Has Changed");



            if (notFoundNode is not null)
            {
                contentRequest.SetPublishedContent(notFoundNode);
            }

            // Return true or false depending on whether our custom 404 page was found
            return Task.FromResult(contentRequest.PublishedContent is not null);
        }
    }
}