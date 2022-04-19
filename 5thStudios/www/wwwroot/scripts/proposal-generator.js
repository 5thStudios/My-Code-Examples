$(window).load(function () {
    // INSTANTIATE VARIABLES
    //=================================================
    //Hidden Fields
    var hfldHours = $('#hfldHours');
    var hfldWeeks = $('#hfldWeeks');
    var hfldCostPerHour = $('#hfldCostPerHour');
    var hfldTotalCost = $('#hfldTotalCost');
    var hfldProposalValues = $('form.formulate__form input[type=hidden]');

    //
    var accPanels = $('.accordion > dd');
    var accHandles = $('.accordion > dt > a');
    var accButtons = $('.accordion input[type=button]');
    var yesNoControllers = $('.yesNoController');
    var yesNoControls = $('.yesNoControl');
    var costPerHour = parseFloat(hfldCostPerHour.val());
    var totalHours = 0;
    var pageCount = 0;
    var allInputs = $('.accordion input:not(:button)');
    var discounted = false;

    //Site Architecture
    var rblTaxonomy_0 = $('#rblTaxonomy_0');
    var rblTaxonomy_1 = $('#rblTaxonomy_1');
    var txbPageCount = $('#txbPageCount');

    //Design
    var rblExistingWebsite_0 = $('#rblExistingWebsite_0');
    var rblExistingWebsite_1 = $('#rblExistingWebsite_1');
    var rblExistingWebsiteOptions_0 = $('#rblExistingWebsiteOptions_0');
    var rblExistingWebsiteOptions_1 = $('#rblExistingWebsiteOptions_1');
    var rblExistingWebsiteOptions_2 = $('#rblExistingWebsiteOptions_2');
    var rblExistingWebsiteOptions_3 = $('#rblExistingWebsiteOptions_3');
    var rblExistingWireframe_0 = $('#rblExistingWireframe_0');
    var rblExistingWireframe_1 = $('#rblExistingWireframe_1');
    var rblNewWebsite_0 = $('#rblNewWebsite_0');
    var rblNewWebsite_1 = $('#rblNewWebsite_1');
    var rblNewWebsite_2 = $('#rblNewWebsite_2');
    var rblNewWebsite_3 = $('#rblNewWebsite_3');

    //Website Type
    var rblWebsiteType_0 = $('#rblWebsiteType_0');
    var rblWebsiteType_1 = $('#rblWebsiteType_1');
    var rblWebsiteType_2 = $('#rblWebsiteType_2');
    var rblWebsiteType_3 = $('#rblWebsiteType_3');
    var rblWebsiteType_4 = $('#rblWebsiteType_4');
    var rblWebsiteType_5 = $('#rblWebsiteType_5');
    var rblWebsiteType_6 = $('#rblWebsiteType_6');
    var rblWebsiteType_7 = $('#rblWebsiteType_7');
    var rblWebsiteType_8 = $('#rblWebsiteType_8');
    var rblWebsiteType_9 = $('#rblWebsiteType_9');
    var rblWebsiteType_10 = $('#rblWebsiteType_10');
    var rblWebsiteType_11 = $('#rblWebsiteType_11');
    var rblWebsiteType_12 = $('#rblWebsiteType_12');
    var rblWebsiteType_13 = $('#rblWebsiteType_13');
    var txbWebsiteType_Other = $('#txbWebsiteType_Other');
    var rblMobile_0 = $('#rblMobile_0');
    var rblMobile_1 = $('#rblMobile_1');

    //Domain Name
    var rblDomainName_0 = $('#rblDomainName_0');
    var rblDomainName_1 = $('#rblDomainName_1');
    var rblDomainName_2 = $('#rblDomainName_2');

    //Web Hosting
    var rblWebHosting_0 = $('#rblWebHosting_0');
    var rblWebHosting_1 = $('#rblWebHosting_1');
    var rblSSL_0 = $('#rblSSL_0');
    var rblSSL_1 = $('#rblSSL_1');

    //Content Management
    var rblMigratingContent_0 = $('#rblMigratingContent_0');
    var rblMigratingContent_1 = $('#rblMigratingContent_1');
    var rblProvideImages_0 = $('#rblProvideImages_0');
    var rblProvideImages_1 = $('#rblProvideImages_1');
    var rblPurchaseImages_0 = $('#rblPurchaseImages_0');
    var rblPurchaseImages_1 = $('#rblPurchaseImages_1');

    //Website Components
    var cbForms = $('#cbForms');
    var cbListPgs = $('#cbListPgs');
    var cbBlogs = $('#cbBlogs');
    var cbPostOnBlog = $('#cbPostOnBlog');
    var cbCommentOnBlog = $('#cbCommentOnBlog');
    var cbSEO = $('#cbSEO');
    var cbVideos = $('#cbVideos');
    var cbPodcasts = $('#cbPodcasts');
    var cbDocuments = $('#cbDocuments');
    var cbMemberships = $('#cbMemberships');
    var cbLockedContent = $('#cbLockedContent');
    var cbMemberAccts = $('#cbMemberAccts');
    var cbDonations = $('#cbDonations');
    var cbPurchase = $('#cbPurchase');
    var cbEmailSignups = $('#cbEmailSignups');
    var cbSearch = $('#cbSearch');
    var cbSitemap = $('#cbSitemap');
    var cbEmails = $('#cbEmails');

    //Social Media
    var cbFollowUs = $('#cbFollowUs');
    var cbShareUs = $('#cbShareUs');

    //Estimate
    var lblWeeks = $('#lblWeeks');
    var lblCosts = $('#lblCosts');




    // EVENTS
    //=================================================
    accHandles.click(function () {
        //Obtain the next panel
        $this = $(this);
        $target = $this.parent().next();

        if (!$target.hasClass('active')) {
            accPanels.removeClass('active').slideUp();
            $target.addClass('active').slideDown();
        }

        return false;
    });
    accButtons.click(function () {
        //Obtain the next panel
        $this = $(this);
        var index = accButtons.index($this);
        accHandles.eq(index + 1).click();

        return false;
    });
    yesNoControllers.click(function () {
        showHideControllers();
    });
    allInputs.click(function () { updateEstimate(); });




    // METHODS
    //=================================================
    function showHideControllers() {
        //
        yesNoControls.hide();
        //
        $(yesNoControllers).each(function () {
            var $this = $(this);
            if ($this.prop('checked')) {
                var val = $this.val();
                var controllername = $this.data('controllername');
                $('.yesNoControl.' + val + '.' + controllername).show();
            }
        })
    }
    function updateEstimate() {
        //Reset variables
        totalHours = 0;
        pageCount = 0;
        doctypes = 0;


        //Scoped Methods
        function update_SiteArchitecture() {
            //               //Site Architecture
            if (rblTaxonomy_0.prop('checked')) totalHours = totalHours + parseInt(rblTaxonomy_0.val());
            if (rblTaxonomy_1.prop('checked')) totalHours = totalHours + parseInt(rblTaxonomy_1.val());
            pageCount = parseInt(txbPageCount.val());

            //If there are more doctypes than pages, update page count
            if (doctypes > pageCount) pageCount = doctypes;            
        }
        function update_Design() {
            //               //Design
            if (rblExistingWebsite_0.prop('checked')) {
                //	YES
                if (rblExistingWebsiteOptions_0.prop('checked')) totalHours = totalHours + parseInt(rblExistingWebsiteOptions_0.val());
                if (rblExistingWebsiteOptions_1.prop('checked')) totalHours = totalHours + parseInt(rblExistingWebsiteOptions_1.val());
                if (rblExistingWebsiteOptions_2.prop('checked')) totalHours = totalHours + parseInt(rblExistingWebsiteOptions_2.val());
                if (rblExistingWebsiteOptions_3.prop('checked')) totalHours = totalHours + parseInt(rblExistingWebsiteOptions_3.val());
                if (rblExistingWireframe_0.prop('checked')) totalHours = totalHours + parseInt(rblExistingWireframe_0.val());
                if (rblExistingWireframe_1.prop('checked')) totalHours = totalHours + parseInt(rblExistingWireframe_1.val());
            }
            else if (rblExistingWebsite_1.prop('checked')) {
                //	NO
                if (rblNewWebsite_0.prop('checked')) totalHours = totalHours + parseInt(rblNewWebsite_0.val());
                if (rblNewWebsite_1.prop('checked')) totalHours = totalHours + parseInt(rblNewWebsite_1.val());
                if (rblNewWebsite_2.prop('checked')) totalHours = totalHours + parseInt(rblNewWebsite_2.val());
                if (rblNewWebsite_3.prop('checked')) totalHours = totalHours + parseInt(rblNewWebsite_3.val());
            }
        }
        function update_WebsiteType() {
            //Initialize multiplier
            var multiplier = 1;

            //Get the Website Type's multiplier value
            if (rblWebsiteType_0.prop('checked')) multiplier = parseFloat(rblWebsiteType_0.val());
            if (rblWebsiteType_1.prop('checked')) multiplier = parseFloat(rblWebsiteType_1.val());
            if (rblWebsiteType_2.prop('checked')) multiplier = parseFloat(rblWebsiteType_2.val());
            if (rblWebsiteType_3.prop('checked')) multiplier = parseFloat(rblWebsiteType_3.val());
            if (rblWebsiteType_4.prop('checked')) multiplier = parseFloat(rblWebsiteType_4.val());
            if (rblWebsiteType_5.prop('checked')) multiplier = parseFloat(rblWebsiteType_5.val());
            if (rblWebsiteType_6.prop('checked')) multiplier = parseFloat(rblWebsiteType_6.val());
            if (rblWebsiteType_7.prop('checked')) multiplier = parseFloat(rblWebsiteType_7.val());
            if (rblWebsiteType_8.prop('checked')) multiplier = parseFloat(rblWebsiteType_8.val());
            if (rblWebsiteType_9.prop('checked')) multiplier = parseFloat(rblWebsiteType_9.val());
            if (rblWebsiteType_10.prop('checked')) multiplier = parseFloat(rblWebsiteType_10.val());
            if (rblWebsiteType_11.prop('checked')) multiplier = parseFloat(rblWebsiteType_11.val());
            if (rblWebsiteType_12.prop('checked')) multiplier = parseFloat(rblWebsiteType_12.val());

            //Update the cost per hour based upon the website type
            costPerHour = (parseInt(hfldCostPerHour.val()) * multiplier);

            //Mobile devices
            if (rblMobile_0.prop('checked')) totalHours = totalHours + parseInt(rblMobile_0.val());
            if (rblMobile_1.prop('checked')) totalHours = totalHours + parseInt(rblMobile_1.val());
        }
        function update_DomainName() {
            ////Domain Name
            if (rblDomainName_0.prop('checked')) totalHours = totalHours + parseInt(rblDomainName_0.val());
            if (rblDomainName_1.prop('checked')) totalHours = totalHours + parseInt(rblDomainName_1.val());
            if (rblDomainName_2.prop('checked')) totalHours = totalHours + parseInt(rblDomainName_2.val());
        }
        function update_WebHosting() {
            ////Web Hosting
            if (rblWebHosting_0.prop('checked')) totalHours = totalHours + parseInt(rblWebHosting_0.val());
            if (rblWebHosting_1.prop('checked')) totalHours = totalHours + parseInt(rblWebHosting_1.val());
            if (rblSSL_0.prop('checked')) totalHours = totalHours + parseInt(rblSSL_0.val());
            if (rblSSL_1.prop('checked')) totalHours = totalHours + parseInt(rblSSL_1.val());
        }
        function update_ContentManagement() {
            ////Content Management
            if (rblMigratingContent_0.prop('checked')) { totalHours = totalHours + pageCount; }
            if (rblProvideImages_0.prop('checked')) { totalHours = totalHours + Math.ceil(pageCount / 2); }
            if (rblPurchaseImages_0.prop('checked')) { totalHours = totalHours + pageCount; }
        }
        function update_WebsiteComponents() {
            ////Website Components
            if (cbForms.prop('checked')) totalHours = totalHours + parseInt(cbForms.val());
            if (cbListPgs.prop('checked')) totalHours = totalHours + parseInt(cbListPgs.val());
            if (cbBlogs.prop('checked')) {
                totalHours = totalHours + parseInt(cbBlogs.val());
                if (cbPostOnBlog.prop('checked')) totalHours = totalHours + parseInt(cbPostOnBlog.val());
                if (cbCommentOnBlog.prop('checked')) totalHours = totalHours + parseInt(cbCommentOnBlog.val());
            }
            if (cbSEO.prop('checked')) totalHours = totalHours + parseInt(cbSEO.val());
            if (cbVideos.prop('checked')) totalHours = totalHours + parseInt(cbVideos.val());
            if (cbPodcasts.prop('checked')) totalHours = totalHours + parseInt(cbPodcasts.val());
            if (cbDocuments.prop('checked')) totalHours = totalHours + parseInt(cbDocuments.val());
            if (cbMemberships.prop('checked')) {
                totalHours = totalHours + parseInt(cbMemberships.val());
                if (cbLockedContent.prop('checked')) totalHours = totalHours + parseInt(cbLockedContent.val());
                if (cbMemberAccts.prop('checked')) totalHours = totalHours + parseInt(cbMemberAccts.val());
            }
            if (cbDonations.prop('checked')) totalHours = totalHours + parseInt(cbDonations.val());
            if (cbPurchase.prop('checked')) totalHours = totalHours + parseInt(cbPurchase.val());
            if (cbEmailSignups.prop('checked')) totalHours = totalHours + parseInt(cbEmailSignups.val());
            if (cbSearch.prop('checked')) totalHours = totalHours + parseInt(cbSearch.val());
            if (cbSitemap.prop('checked')) totalHours = totalHours + parseInt(cbSitemap.val());
            if (cbEmails.prop('checked')) totalHours = totalHours + parseInt(cbEmails.val());
        }
        function update_SocialMedia() {
            ////Social Media
            if (cbFollowUs.prop('checked')) totalHours = totalHours + parseInt(cbFollowUs.val());
            if (cbShareUs.prop('checked')) totalHours = totalHours + parseInt(cbShareUs.val());
        }
        function update_DoctypeCount() {
            ////Website Components
            if (cbForms.prop('checked')) doctypes = doctypes + 1;
            if (cbListPgs.prop('checked')) doctypes = doctypes + 2;
            if (cbBlogs.prop('checked')) {
                doctypes = doctypes + 2;
                if (cbPostOnBlog.prop('checked')) doctypes = doctypes + 1;
                if (cbCommentOnBlog.prop('checked')) doctypes = doctypes + 1;
            }
            if (cbVideos.prop('checked')) doctypes = doctypes + 2;
            if (cbPodcasts.prop('checked')) doctypes = doctypes + 2;
            if (cbMemberships.prop('checked')) {
                doctypes = doctypes + 1;
                if (cbLockedContent.prop('checked')) doctypes = doctypes + 1;
                if (cbMemberAccts.prop('checked')) doctypes = doctypes + 1;
            }
            if (cbDonations.prop('checked')) doctypes = doctypes + 1;
            if (cbPurchase.prop('checked')) doctypes = doctypes + 3;
            if (cbEmailSignups.prop('checked')) doctypes = doctypes + 1;
            if (cbSearch.prop('checked')) doctypes = doctypes + 1;
            if (cbSitemap.prop('checked')) doctypes = doctypes + 1;
        }
        function update_UpdateEstimate() {
            //Hidden Fields
            hfldHours.val(totalHours);
            hfldWeeks.val(Math.ceil(totalHours / 30));
            hfldTotalCost.val(totalHours * costPerHour);

            //Estimate
            lblWeeks.text(hfldWeeks.val());
            lblCosts.text(numberWithCommas(parseInt(totalHours * costPerHour)));

            //console.log('================================================================');
            //console.log('Total Hours: ' + totalHours);
            //console.log('Cost/Hour: ' + costPerHour);
            //console.log('Total Cost: $' + hfldTotalCost.val());

        }
        function saveDataToJson() {
            var proposalJson = {
                "Site Architecture": {
                    "I will be providing the site’s taxonomy": rblTaxonomy_0.prop('checked'),
                    "Create a site taxonomy for me": rblTaxonomy_1.prop('checked'),
                    "How many pages will the site contain": txbPageCount.val(),
                    "Number of Doctypes": doctypes
                },

                "Design": {
                    "I have an existing website or web design": rblExistingWebsite_0.prop('checked'),
                    "Yes": {
                        "Reusing the existing design": rblExistingWebsiteOptions_0.prop('checked'),
                        "Having us upgrade the existing design": rblExistingWebsiteOptions_1.prop('checked'),
                        "Having us create a new design": rblExistingWebsiteOptions_2.prop('checked'),
                        "Providing us with the new site’s design layout": rblExistingWebsiteOptions_3.prop('checked'),
                        "We have an existing wireframe": rblExistingWireframe_0.prop('checked')
                    },
                    "I do not have an existing website or web design": rblExistingWebsite_1.prop('checked'),
                    "No": {
                        "Having us create a new site design & wireframe": rblNewWebsite_0.prop('checked'),
                        "Providing us with the new site’s design and wireframe": rblNewWebsite_1.prop('checked'),
                        "Providing us with the new site’s design only": rblNewWebsite_2.prop('checked'),
                        "Providing us with the new site’s wireframe only": rblNewWebsite_3.prop('checked'),
                    }
                },

                "Website Type": {
                    "Blog": rblWebsiteType_5.prop('checked'),
                    "Brochure Website": rblWebsiteType_2.prop('checked'),
                    "Business Directory": rblWebsiteType_9.prop('checked'),
                    "Business Website or Web Portal": rblWebsiteType_6.prop('checked'),
                    "E-commerce Website": rblWebsiteType_12.prop('checked'),
                    "Infopreneur Website": rblWebsiteType_1.prop('checked'),
                    "Job Board": rblWebsiteType_8.prop('checked'),
                    "Media or Entertainment Website": rblWebsiteType_7.prop('checked'),
                    "Nonprofit or Religious Website": rblWebsiteType_3.prop('checked'),
                    "Personal Website": rblWebsiteType_0.prop('checked'),
                    "Podcasting website": rblWebsiteType_11.prop('checked'),
                    "Portfolio Website": rblWebsiteType_4.prop('checked'),
                    "Wiki or Community Forum Website": rblWebsiteType_10.prop('checked'),
                    "Other": rblWebsiteType_13.prop('checked'),
                    "Description": txbWebsiteType_Other.val()
                },

                "Mobile Devices": {
                    "Make the website adjust responsively to mobile browsers": rblMobile_0.prop('checked'),
                    "Create a mobile app that can be installed on mobile devices": rblMobile_1.prop('checked')
                },

                "Domain Name": {
                    "I have an existing domain name": rblDomainName_0.prop('checked'),
                    "I will purchase the domain name": rblDomainName_1.prop('checked'),
                    "Please purchase the domain name for me": rblDomainName_2.prop('checked')
                },

                "Web Hosting": {
                    "I will provide a host site for you": rblWebHosting_0.prop('checked'),
                    "Please purchase the proper hosting package for me.": rblWebHosting_1.prop('checked'),
                    "Is an SSL needed": rblSSL_0.prop('checked')
                },

                "Content Management": {
                    "Will we be entering or migrating content to the new site?": rblMigratingContent_0.prop('checked'),
                    "Will you be providing us with any needed images?": rblProvideImages_0.prop('checked'),
                    "Will we be permitted to purchase any needed images?": rblPurchaseImages_0.prop('checked')
                },

                "Website Components": {
                    "Accept Donations": cbDonations.prop('checked'),
                    "Accept email signups": cbEmailSignups.prop('checked'),
                    "Allow purchase of products or services": cbPurchase.prop('checked'),
                    "Blogs": cbBlogs.prop('checked'),
                    "Blog": {
                        "Will readers be able to post on the blog?": cbPostOnBlog.prop('checked'),
                        "Will readers be able to comment on the blog?": cbCommentOnBlog.prop('checked')
                    },
                    "Emails": cbEmails.prop('checked'),
                    "Forms": cbForms.prop('checked'),
                    "List Pages": cbListPgs.prop('checked'),
                    "Memberships & Login Capabilities": cbMemberships.prop('checked'),
                    "Membership": {
                        "Will some content need to be locked?": cbLockedContent.prop('checked'),
                        "Will members need to create & edit their accounts?": cbMemberAccts.prop('checked')
                    },
                    "PDFs & Documents": cbDocuments.prop('checked'),
                    "Podcasts": cbPodcasts.prop('checked'),
                    "Search page": cbSearch.prop('checked'),
                    "SEO Tools": cbSEO.prop('checked'),
                    "Site Map": cbSitemap.prop('checked'),
                    "Videos": cbVideos.prop('checked')
                },

                "Social Media": {
                    "Add Follow Us links to site": cbFollowUs.prop('checked'),
                    "Allow my audience to Share my site on social medias": cbShareUs.prop('checked')
                },

                "Total hours": hfldHours.val(),
                "How many weeks": hfldWeeks.val(),
                "Project Cost": hfldTotalCost.val()
            }
            //Save json in hidden field
            hfldProposalValues.val(JSON.stringify(proposalJson));

            //console.log(proposalJson);
            //console.log(hfldProposalValues.val());
        }
        function numberWithCommas(x) {
            return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
       


        //Scoped method calls
        update_DoctypeCount();
        update_SiteArchitecture();
        update_Design();
        update_WebsiteType();
        update_DomainName();
        update_WebHosting();
        update_ContentManagement();
        update_WebsiteComponents();
        update_SocialMedia();
        update_UpdateEstimate();
        saveDataToJson();
    }

    // INIT
    //=================================================
    //
    accPanels.hide();
    //Open 1st accordion panel
    $('.accordion > dt:first-of-type > a').click();
    //
    showHideControllers();
    //
    updateEstimate();
});