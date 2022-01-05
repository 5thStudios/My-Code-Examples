//==================================================
//  Manage Account
//==================================================
jQuery(function ($) {
    function jsManageAcct() {
        //Instantiate variables
        var filters = $('.filters li:not(.inactive)');
        var inactiveFilters = $('.filters li.inactive a');
        var pnlEditAcct = $('#pnlEditAcct');
        var pnlAddEditIlluminationStory = $('#pnlAddEditIlluminationStory');
        var pnlAddIlluminationStory = $('#pnlAddIlluminationStory');
        var pnlIlluminationStory = $('#pnlIlluminationStory');

        //Handles
        $(filters).click(function () {
            //Instantiate variable
            var filter = $(this);

            //Set the filter buttons
            filters.removeClass("active");
            filter.addClass("active");
        });
        $(inactiveFilters).click(function (e) {
            e.preventDefault();
        });


        function showActiveBtn() {
            filters.removeClass("active");
            if (pnlEditAcct.length === 1) { filters.eq(0).addClass("active"); }
            if (pnlAddEditIlluminationStory.length === 1) { filters.eq(1).addClass("active"); }
            if (pnlAddIlluminationStory.length === 1) { filters.eq(1).addClass("active"); }
            if (pnlIlluminationStory.length === 1) { filters.eq(1).addClass("active"); }
        }
        showActiveBtn();
    }

    try {
        jsManageAcct();
    }
    catch (err) {
        console.log('ERROR: [jsManageAcct] ' + err.message);
    }
});