using ECMC_Umbraco.Models;
using ECMC_Umbraco.ViewModel;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace UmbracoProject.Models
{
    public class ListFilters
    {
        public FilterViewModel GenerateFilters(ListViewModel lstVmodel, IPublishedContent ipPg)
        {
            //Determine which filters to display on screen
            if (lstVmodel.LstListItems.Where(x => x.LstAudience != null && x.LstAudience.Count > 0).Any())
                lstVmodel.filterViewModel.ShowAudienceFilter = true;
            if (lstVmodel.LstListItems.Where(x => x.LstStaff != null && x.LstStaff.Count > 0).Any())
                lstVmodel.filterViewModel.ShowStaffFilter = true;
            if (lstVmodel.LstListItems.Where(x => x.LstAreasOfInterest != null && x.LstAreasOfInterest.Count > 0).Any())
                lstVmodel.filterViewModel.ShowAreaOfInterestFilter = true;


            //Loop through all records to obtain filters
            foreach (ListItemViewModel record in lstVmodel.LstListItems)
            {

                //Create list of all filters by Area of Interest
                if (record.LstAreasOfInterest != null)
                {
                    foreach (string tag in record.LstAreasOfInterest)
                    {
                        if (!lstVmodel.filterViewModel.LstAreaOfInterestFilter.Contains(tag))
                        {
                            lstVmodel.filterViewModel.LstAreaOfInterestFilter.Add(tag);
                            lstVmodel.filterViewModel.FilterCount++;
                        }
                    }
                }


                //Create list of all filters by 
                if (record.LstAudience != null)
                {
                    foreach (string tag in record.LstAudience)
                    {
                        if (!lstVmodel.filterViewModel.LstAudienceFilter.Contains(tag))
                        {
                            lstVmodel.filterViewModel.LstAudienceFilter.Add(tag);
                            lstVmodel.filterViewModel.FilterCount++;
                        }
                    }
                }


                //Create list of all filters by 
                if (record.LstStaff != null)
                {
                    foreach (string tag in record.LstStaff)
                    {
                        if (!lstVmodel.filterViewModel.LstStaffFilter.Contains(tag))
                        {
                            lstVmodel.filterViewModel.LstStaffFilter.Add(tag);
                            lstVmodel.filterViewModel.FilterCount++;
                        }
                    }
                }


            }


            /*
             THE ABOVE CODE SHOWS ONLY FILTER OPTIONS SELECTED AS TAGS IN THE CARDS.
             THE BELOW CODE SHOWS ALL FILTER OPTIONS BASED ON AVAILABLE TAGS.
             */

            ////Obtain filter data from page
            //var AdminTags = ipPg.Root().Siblings()?
            //    .Where(x => x.ContentType.Alias == Common.Doctype.AdministrativeSettings).FirstOrDefault()?
            //    .Descendants<Tags>().ToList();


            ////Create filter list IF filter is to be displayed
            //if (AdminTags != null)
            //{
            //    foreach (var ipFilter in AdminTags)
            //    {
            //        switch (ipFilter.Name)
            //        {
            //            case Common.TagGroup.AreaOfInterest:
            //                if (lstVmodel.filterViewModel.ShowAreaOfInterestFilter)
            //                {
            //                    lstVmodel.filterViewModel.LstAreaOfInterestFilter = new List<string>();
            //                    foreach (var ipTag in ipFilter.Children)
            //                        lstVmodel.filterViewModel.LstAreaOfInterestFilter.Add(ipTag.Name);
            //                    lstVmodel.filterViewModel.FilterCount++;
            //                }
            //                break;

            //            case Common.TagGroup.Audience:
            //                if (lstVmodel.filterViewModel.ShowAudienceFilter)
            //                {
            //                    lstVmodel.filterViewModel.LstAudienceFilter = new List<string>();
            //                    foreach (var ipTag in ipFilter.Children)
            //                        lstVmodel.filterViewModel.LstAudienceFilter.Add(ipTag.Name);
            //                    lstVmodel.filterViewModel.FilterCount++;
            //                }
            //                break;

            //            case Common.TagGroup.Staff:
            //                if (lstVmodel.filterViewModel.ShowStaffFilter)
            //                {
            //                    lstVmodel.filterViewModel.LstStaffFilter = new List<string>();
            //                    foreach (var ipTag in ipFilter.Children)
            //                        lstVmodel.filterViewModel.LstStaffFilter.Add(ipTag.Name);
            //                    lstVmodel.filterViewModel.FilterCount++;
            //                }
            //                break;

            //            default:
            //                break;
            //        }
            //    }
            //}





            //Return filter lists
            return lstVmodel.filterViewModel;
        }

        public ListFilters() { }
    }
}
