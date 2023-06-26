//==================================================
//  Pagination
//  pagination.js
//==================================================
jQuery(function ($) {

    function jsPagination() {
        // Instantiate variables
        var setSize = 5;
        var existingPageIDs = [];
        var index = 1;
        var pageIdSets = [];
        var currentSet = [];
        var paginationLst = $(".paginationLst");
        var pageNo = paginationLst.eq(0).find("#pageNo").val();
        var totalPages = paginationLst.eq(0).find("#totalPages").val();
        var pageNoLink = $(".paginationLst .page-no");
        var prevSetIcon = $(".paginationLst .pagination-previous-set");
        var nextSetIcon = $(".paginationLst .pagination-next-set");
        var prevSetBtn = $(".paginationLst .pagination-previous-set a");
        var nextSetBtn = $(".paginationLst .pagination-next-set a");
        var paginationFirst = $(".paginationLst .pagination-first");
        var prevPage = $(".paginationLst .pagination-previous");
        var nextPage = $(".paginationLst .pagination-next");
        var paginationLast = $(".paginationLst .pagination-last");

        //console.log('pageNo: ' + pageNo);
        //console.log('totalPages: ' + totalPages);


        //IF LESS THAN SET SIZE, SHOW ALL PAGES
        if (totalPages === "0") {
            paginationLst.hide();
        }
        else if (totalPages === "1") {
            paginationLst.hide();
        }
        else if (totalPages <= setSize) {
            //SHOW ALL LINKS
            pageNoLink.show();
            paginationFirst.show();
            paginationLast.show();
            prevSetIcon.hide();
            nextSetIcon.hide();
        }
        else {
            //CREATE ARRAY OF PAGE NUMBERS
            while (index <= totalPages) {
                existingPageIDs.push(index);
                index++;
            }
            //console.log(existingPageIDs);


            //SPLIT ARRAY OF PAGE NUMBERS INTO GROUPS
            while (existingPageIDs.length > 0)
                pageIdSets.push(existingPageIDs.splice(0, setSize));
            //console.log(pageIdSets);


            //FIND WHICH ARRAY SET THE CURRENT PAGE EXISTS IN
            currentSet = parseInt((pageNo - 1) / setSize);
            //console.log('currentSet: ' + currentSet);
            //console.log('pageIdSets: ' + pageIdSets.length);


            //HIDE ALL BUT PAGES LISTED IN CURRENT SET
            pageNoLink.hide();
            for (var i = 0; i < pageIdSets[currentSet].length; i++) {
                pageNoLink.filter('[data-pageno="' + pageIdSets[currentSet][i] + '"]').show();
            }


            //SHOW SET ICONS IF NEEDED
            if (currentSet > 0) { prevSetIcon.show(); }
            if (currentSet < (pageIdSets.length - 1)) { nextSetIcon.show(); }


            //SHOW FIRST/LAST LINKS IF NEEDED
            if (parseInt(pageNo) > 1) { paginationFirst.show(); }
            if (parseInt(pageNo) < parseInt(totalPages)) { paginationLast.show(); }


            //SHOW PREV/NEXT PAGE LINKS IF NEEDED
            prevPage.hide();
            nextPage.hide();
            if (parseInt(pageNo) > 1) { prevPage.show(); }
            if (parseInt(pageNo) < parseInt(totalPages)) { nextPage.show(); }
        }


        //HANDLES
        $(prevSetBtn).click(function () {
            pageNoLink.hide();
            currentSet = currentSet - 1;
            for (var i = 0; i < pageIdSets[currentSet].length; i++) {
                pageNoLink.filter('[data-pageno="' + pageIdSets[currentSet][i] + '"]').show();
            }
            //SHOW SET ICONS IF NEEDED
            prevSetIcon.hide();
            nextSetIcon.hide();
            if (currentSet > 0) { prevSetIcon.show(); }
            if (currentSet < (pageIdSets.length - 1)) { nextSetIcon.show(); }
        });
        $(nextSetBtn).click(function () {
            pageNoLink.hide();
            currentSet = currentSet + 1;
            for (var i = 0; i < pageIdSets[currentSet].length; i++) {
                pageNoLink.filter('[data-pageno="' + pageIdSets[currentSet][i] + '"]').show();
            }
            //SHOW SET ICONS IF NEEDED
            prevSetIcon.hide();
            nextSetIcon.hide();
            if (currentSet > 0) { prevSetIcon.show(); }
            if (currentSet < (pageIdSets.length - 1)) { nextSetIcon.show(); }
        });


    }

    //Run only if element exists
    try {
        if ($('.paginationLst').length > 0) { jsPagination(); }
    }
    catch (err) {
        console.log('ERROR: [jsPagination] ' + err.message);
    }
});













//==================================================
//  Pagination
//==================================================
//jQuery(function ($) {
//    try {
//        function jsPagination() {
//            // Instantiate variables
//            var items = $(".viewItem");
//            var numItems = items.length;
//            var perPage = 10;

//            // Only show the first # (or first `per_page`) items initially.
//            items.slice(perPage).hide();

//            // Now setup the pagination using the `.pagination-page` div.
//            $(".pagination-page").pagination({
//                items: numItems,
//                itemsOnPage: perPage,
//                cssStyle: "light-theme",
//                // This is the actual page changing functionality.
//                onPageClick: function (pageNumber) {
//                    // We need to show and hide `tr`s appropriately.
//                    var showFrom = perPage * (pageNumber - 1);
//                    var showTo = showFrom + perPage;

//                    // We'll first hide everything...
//                    items.hide()
//                        // ... and then only show the appropriate rows.
//                        .slice(showFrom, showTo).show();
//                }
//            });

//            // EDIT: Let's cover URL fragments (i.e. #page-3 in the URL).
//            // More thoroughly explained (including the regular expression) in:
//            // https://github.com/bilalakil/bin/tree/master/simplepagination/page-fragment/index.html

//            // We'll create a function to check the URL fragment
//            // and trigger a change of page accordingly.
//            function checkFragment() {
//                // If there's no hash, treat it like page 1.
//                var hash = window.location.hash || "#page-1";

//                // We'll use a regular expression to check the hash string.
//                hash = hash.match(/^#page-(\d+)$/);

//                if (hash) {
//                    // The `selectPage` function is described in the documentation.
//                    // We've captured the page number in a regex group: `(\d+)`.
//                    $(".pagination-page").pagination("selectPage", parseInt(hash[1]));
//                }
//            };

//            // We'll call this function whenever back/forward is pressed...
//            $(window).bind("popstate", checkFragment);

//            // ... and we'll also call it when the page has loaded
//            // (which is right now).
//            checkFragment();


//            //Add 'end' class to final gridview item.
//            var lastGridviewItem = $('div.gridViewItem:last-child');
//            lastGridviewItem.addClass("end");
//        };

//        //Run only if element exists
//        if ($('.pagination-page').length > 0) { jsPagination(); }
//        //jsPagination();
//    }
//    catch (err) {
//        console.log('ERROR: [jsPagination] ' + err.message);
//    }
//});