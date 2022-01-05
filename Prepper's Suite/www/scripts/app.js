//  INSTANTIATE ZURB FOUNDATION
//========================================================
jQuery(function ($) {
    $(document).foundation();
});


//  AUTORESIZE INPUT FIELDS
//========================================================
jQuery(function ($) {
    $('textarea.autosize').on('input', function () {
        this.style.height = 'auto';
        this.style.height = (this.scrollHeight) + 'px';
    });
});


//  OPEN SIDE NAV
//========================================================
jQuery(function ($) {
    var aside = $('aside');
    var main = $('main');
    var navOpenBtn = $('#nav-open');
    var boolOpen = false;

    $(navOpenBtn).click(function () {
        if (boolOpen) {
            aside.removeClass("open");
            main.removeClass("open");
        }
        else {
            aside.addClass("open");
            main.addClass("open");
        }

        //
        boolOpen = !boolOpen;
    });
});


//  JQUERY ACCORDIONS
//========================================================
jQuery(function ($) {
    var icons = {
        header: "ui-icon-plus",
        activeHeader: "ui-icon-minus"
    };
    var btnExpandAll = $('#btn-expand-all');
    var btnCollapseAll = $('#btn-collapse-all');

    $(".accordion").accordion({
        collapsible: true,
        icons: icons,
        active: false,
        heightStyle: "content",
        beforeActivate: function (event, ui) {
            // The accordion believes a panel is being opened
            if (ui.newHeader[0]) {
                var currHeader = ui.newHeader;
                var currContent = currHeader.next('.ui-accordion-content');
                // The accordion believes a panel is being closed
            } else {
                var currHeader = ui.oldHeader;
                var currContent = currHeader.next('.ui-accordion-content');
            }
            // Since we've changed the default behavior, this detects the actual status
            var isPanelSelected = currHeader.attr('aria-selected') == 'true';

            // Toggle the panel's header
            currHeader.toggleClass('ui-corner-all', isPanelSelected).toggleClass('ui-accordion-header-active ui-state-active ui-corner-top', !isPanelSelected).attr('aria-selected', ((!isPanelSelected).toString()));

            // Toggle the panel's icon
            currHeader.children('.ui-icon').toggleClass('ui-icon-plus', isPanelSelected).toggleClass('ui-icon-minus', !isPanelSelected);

            // Toggle the panel's content
            currContent.toggleClass('ui-accordion-content-active', !isPanelSelected)
            if (isPanelSelected) { currContent.slideUp(); } else { currContent.slideDown(); }

            return false; // Cancels the default action
        }
    });
    $(btnExpandAll).click(function (e) {
        //
        $(".ui-accordion-header").each(function (i) {
            if ($(this).attr('aria-selected') == 'false') {
                $(".accordion").accordion("option", "active", i);
            }
        });
        console.log('expand all');
        //$('.accordion .ui-accordion-content').css("display", "block").attr('aria-expanded', 'true').attr('aria-hidden', 'false');
        //$('.accordion .ui-accordion-header').removeClass('ui-accordion-header-active ui-state-active');
    })
    $(btnCollapseAll).click(function (e) {
        //
        $(".ui-accordion-header").each(function (i) {
            if ($(this).attr('aria-selected') == 'true') {
                $(".accordion").accordion("option", "active", i);
            }
        });
        console.log('collapse all');
        //$('.accordion .ui-accordion-content').css("display", "none").attr('aria-expanded', 'false').attr('aria-hidden', 'true');
        //$('.accordion .ui-accordion-header').addClass('ui-accordion-header-active ui-state-active');
    })



    //Temp: set all open
    $(".ui-accordion-header").each(function (i) {
        if ($(this).attr('aria-selected') == 'false') {
            $(".accordion").accordion("option", "active", i);
        }
    });

});
jQuery(function ($) {
    var heading = $('.heading');

    //
    $(heading).click(function (e) {
        var isOpen = $(this).data('isopen');

        if (isOpen == 'true') {
            $(this).parent().find('.record').css('display', 'none');
            $(this).data('isopen', 'false');

            $(this).find('.fa-plus').toggle();
            $(this).find('.fa-minus').toggle();
        }
        else {
            $(this).parent().find('.record').css('display', 'flex');
            $(this).data('isopen', 'true');

            $(this).find('.fa-plus').toggle();
            $(this).find('.fa-minus').toggle();
        }

    })
})


//  Toggles
//========================================================
jQuery(function ($) {
    //For each toggle 
    $("[data-mobile-app-toggle]").each(function () {
        var noBtn = $(this).children('.button.no');
        var yesBtn = $(this).children('.button.yes');
        var isActiveClass = 'is-active';
        var input = $(this).children('input');
        var inputValue = input.val();

        //Set to 'yes' if value == true;
        if (inputValue == null) {
            inputValue = 'false';
        }
        var isTrue = inputValue.toLowerCase() == 'true' ? true : false;
        if (is.truthy(isTrue)) {
            $(noBtn).removeClass(isActiveClass);
            $(yesBtn).addClass(isActiveClass);
        }
    });

    //On toggle click
    $('[data-mobile-app-toggle] .button').click(function () {
        var input = $(this).parent().children('input');
        $(this).siblings().removeClass('is-active');
        $(this).addClass('is-active');
        if ($(this).hasClass('no')) {
            input.val(false);
        }
        else {
            input.val(true);
        }
    });



});



//==================================================
//  Functions & Methods
//==================================================
jQuery(function ($) {
    function jsCommon() {
        //  FUEL PANEL
        // Instantiate variables
        var pnlFuel = $("fieldset#pnl-fuel");
        var btnFuel_No = $('.btn-requires-fuel .button:first-of-type');
        var btnFuel_Yes = $('.btn-requires-fuel .button:last-of-type');
        var requiresFuel = $('.btn-requires-fuel input').val();

        //Methods
        $(btnFuel_Yes).click(function (e) {
            console.log('show');
            pnlFuel.show();
        });
        $(btnFuel_No).click(function (e) {
            console.log('hide');
            pnlFuel.hide();
        });

        if (requiresFuel == null) {
            requiresFuel = 'false';
        }
        var blRequiresFuel = (requiresFuel.toLowerCase() == 'true' ? true : false);
        if (is.truthy(blRequiresFuel)) {
            pnlFuel.show();
        }





        //  BATTERY PANEL
        // Instantiate variables
        var pnlBatteries = $("fieldset#pnl-batteries");
        var btnBatteries_No = $('.btn-requires-batteries .button:first-of-type');
        var btnBatteries_Yes = $('.btn-requires-batteries .button:last-of-type');
        var requiresBatteries = $('.btn-requires-batteries input').val();

        //Methods
        $(btnBatteries_Yes).click(function (e) {
            pnlBatteries.show();
        });
        $(btnBatteries_No).click(function (e) {
            pnlBatteries.hide();
        });

        if (requiresBatteries == null) {
            requiresBatteries = 'false';
        }
        var blRequiresBatteries = (requiresBatteries.toLowerCase() == 'true' ? true : false);
        if (is.truthy(blRequiresBatteries)) {
            pnlBatteries.show();
        }





        //  MEASUREMENT SYSTEMS
        // Instantiate variables
        var btnImperial = $('.btn-measurement-system .button:first-of-type');
        var btnMetric = $('.btn-measurement-system .button:last-of-type');
        var inpMeasurementSystem = $('.btn-measurement-system input');
        var ddlMeasurementTypes_Select = $('.ddlMeasurementTypes select');
        var ddlMeasurementTypes_Options = $('.ddlMeasurementTypes select option');

        //Methods
        $(btnImperial).click(function (e) {
            inpMeasurementSystem.val($(this).data("system-id"));
            resetDDL($(this).data("system-id"));
        });
        $(btnMetric).click(function (e) {
            inpMeasurementSystem.val($(this).data("system-id"));
            resetDDL($(this).data("system-id"));
        });
        function resetDDL(systemId) {
            $(ddlMeasurementTypes_Options).hide();
            $(ddlMeasurementTypes_Options).first().show();
            $(ddlMeasurementTypes_Options).each(function () {
                if ($(this).data('measurement-system-id') == systemId) {
                    $(this).show();
                }
            });
            $(ddlMeasurementTypes_Options).first().prop("selected", true);
        }

        //Initialize
        if (inpMeasurementSystem.val() == "" || inpMeasurementSystem.val() == "0") {
            $(btnImperial).click();
        }
        else if (inpMeasurementSystem.val() == $(btnImperial).data("system-id")) {
            $(btnImperial).click();
        }
        else {
            $(btnMetric).click();
        }
        if ($(ddlMeasurementTypes_Select).data("measurement-type-id") != "") {
            var id = $(ddlMeasurementTypes_Select).data("measurement-type-id");
            $(ddlMeasurementTypes_Select).val(id);
        }

        //data-measurement-type-id



    };
    function jsbarcodePnl() {
        // Instantiate variables
        var btnInputTypes = $(".btn-input-type a.button");
        var pnlBarcodeLookup = $(".barcode-pnl");
        var BtnsMeasurementSystems = $('.btn-measurement-system .button')
        //Hidden Fields
        var hfldUpc = $("#hfldUpc").val();
        var hfldProductLabel = $("#hfldProductLabel").val();
        var hfldServingsPerContainer = $("#hfldServingsPerContainer").val();
        var hfldServingSize = $("#hfldServingSize").val();
        var hfldUnits = $("#hfldUnits").val() * hfldServingsPerContainer;
        var hfldJson = $("#NewItem_JsonData").val();
        //Input Fields
        var txbItemBarcode = $('#NewItem_Barcode');
        var txbItemName = $('#NewItem_Name');
        var rbtnMeasurementSystemId = $('#NewItem_Volume_MeasurementSystemId');
        var ddlMeasurementType = $('#ddlNewMeasurementTypeId');
        var txbVolumeUnits = $('#NewItem_Volume_Units');



        //Handles
        $(btnInputTypes).click(function (e) {
            //Determine if the barcode panel should appear or not
            var value = $(this).data("value");
            if (value == true) {
                pnlBarcodeLookup.show();
            }
            else {
                pnlBarcodeLookup.hide();
            }
        });


        //Methods
        if (hfldJson.length > 0) {
            //POPULATE DATA ONTO SCREEN IN PROPER FIELDS

            txbItemBarcode.val('');
            txbItemName.val(hfldProductLabel);
            txbVolumeUnits.val(hfldUnits);

            //Find matching dropdown
            var ddlItem = $("#ddlNewMeasurementTypeId option").filter(function () {
                return $(this).text().toLowerCase() == hfldServingSize.toLowerCase();
            })
            //Obtain data from option
            var systemId = ddlItem.data('measurement-system-id');
            var measurementId = ddlItem.val();

            //select system type
            BtnsMeasurementSystems.removeClass('is-active');
            $(BtnsMeasurementSystems).each(function () {
                if ($(this).data('system-id') === systemId) {
                    $(this).addClass('is-active');
                }
            });

            //select measurement type
            $(ddlItem).attr('selected', true);

        }





        //Remember if upc input is selected or not
        var btnManual = $('.upc-inputs .button').first();
        var btnBarcode = $('.upc-inputs .button').last();
        var showBarcode = false;
        if (typeof localStorage['showBarcode'] !== 'undefined' && localStorage['showBarcode'] !== null) {
            showBarcode = localStorage['showBarcode'];
        }
        $(btnManual).click(function () {
            showBarcode = false;
            localStorage['showBarcode'] = false;
        })
        $(btnBarcode).click(function () {
            showBarcode = true;
            localStorage['showBarcode'] = true;
        })
        if (showBarcode == "true") {
            btnManual.removeClass('is-active');
            btnBarcode.addClass('is-active');
            pnlBarcodeLookup.show();
        }




        //Remember if upc input is manual or rapid fire
        var btnSingleInput = $('.input-type-btns .button').first();
        var btnRapidInput = $('.input-type-btns .button').last();
        var hfldRapidInput = $('#RapidInput');
        var isRapidInput = false;
        if (typeof localStorage['isRapidInput'] !== 'undefined' && localStorage['isRapidInput'] !== null) {
            isRapidInput = localStorage['isRapidInput'];
        }
        $(btnSingleInput).click(function () {
            isRapidInput = false;
            localStorage['isRapidInput'] = false;
            hfldRapidInput.val(isRapidInput);
        })
        $(btnRapidInput).click(function () {
            isRapidInput = true;
            localStorage['isRapidInput'] = true;
            hfldRapidInput.val(isRapidInput);
        })
        if (isRapidInput == "true") {
            btnSingleInput.removeClass('is-active');
            btnRapidInput.addClass('is-active');
            hfldRapidInput.val(isRapidInput);
        }












        // Initialize with options
        //https://a.kabachnik.info/onscan-js.html
        onScan.attachTo(document, {
            onScan: function (sCode, iQty) { // Alternative to document.addEventListener('scan')
                console.log(sCode);
                txbItemBarcode.val(sCode);

                //$("button[name='btnLookup']").click(function () { });

                $("button[name='btnLookup']").trigger('click');
            }
        });




        //// Enable scan events for the entire document
        //onScan.attachTo(document);
        //// Register event listener
        //document.addEventListener('scan', function (sScancode, iQuatity) {
        //    console.log(sScancode);
        //    txbItemBarcode.val(sScancode);
        //});





    };
    function jsTabs() {
        // Instantiate variables
        var tabs = $(".tab-buttons .button");
        var tabItems = $('.tab-items .tab');
        var btnAddItem = $('#btn-add-item');
        var ShowUpdateRecord = $('#ShowUpdateRecord').val();


        $(tabs).click(function (e) {
            //console.log($(tabs).index(this));

            //Set Active tab
            tabs.removeClass('active');
            $(this).addClass('active');
            tabItems.hide();
            tabItems.eq($(tabs).index(this)).show();
        });
        $(btnAddItem).click(function (e) {
            ShowAddPnl();
        })
        if (ShowUpdateRecord == 'True') {
            ShowAddPnl();
        }


        function ShowAddPnl() {
            //
            var tabs = $(".tab-buttons .button");
            var tabItems = $('.tab-items .tab');

            //Set Active tab
            tabs.removeClass('active');
            tabs.eq(1).addClass('active');
            tabItems.hide();
            tabItems.eq(1).show();
        }

        //Activate 1st tab
        //tabs.eq(0).addClass('active');


        //TEMP
        tabs.eq(1).addClass('active');
        ShowAddPnl();
    };
    function jsBugoutBags() {
        // Instantiate variables
        var ddlToolsets = $('#ddlToolsets');
        var ddlBugoutLocations = $('#NewItem_LocationId');
        var ddlCategories = $('#ddlCategories');
        var ddlCategoryOptions = $('#ddlCategories option');
        var lstRecords = $('#lstRecords .record');
        var toolsetId = "";
        var categoryId = "";


        //disable all inputs, selects and submit btns
        $('#formPnl :input').attr('disabled', true);
        //$('#formPnl :select').attr('disabled', true);
        //$('#formPnl :submit').attr('disabled', 'true');

        $(ddlBugoutLocations).change(function () {
            //Determine if dropdowns should be active or not.
            if (this.value == "") {
                //disable all inputs, selects and submit btns
                $('#formPnl :input').attr('disabled', true);
                //$('#formPnl :select').attr('disabled', true);
                //$('#formPnl :submit').attr('disabled', 'true');

                //Reset to blank value
                $("#ddlCategories option:selected").prop("selected", false);
                $("#ddlCategories option:first").prop("selected", "selected");
                //Reset to blank value
                $("#ddlToolsets option:selected").prop("selected", false);
                $("#ddlToolsets option:first").prop("selected", "selected");

            }
            else {
                //enable all inputs, selects and submit btns
                $('#formPnl :input').attr('disabled', false);
                //$('#formPnl :select').attr('disabled', false);
                //$('#formPnl :submit').attr('disabled', 'false');
            }

        });
        $(ddlToolsets).change(function () {
            //Reset dropdowns 
            toolsetId = this.value;
            categoryId = "";
            //
            SetCategoryDdl();
            SetRecordList();
        });
        $(ddlCategories).change(function () {
            //Reset dropdowns 
            categoryId = this.value;

            //SetCategoryDdl();
            SetRecordList();
        });


        function SetCategoryDdl() {
            //Hide all options
            ddlCategoryOptions.hide();
            //Show first record
            ddlCategoryOptions.first().show();
            //Show records that match toolset
            $(ddlCategoryOptions).each(function () {
                if ($(this).data('toolid') == toolsetId) {
                    $(this).show();
                }
            });
            //Reset to blank value
            $("#ddlCategories option:selected").prop("selected", false);
            $("#ddlCategories option:first").prop("selected", "selected");
        }
        function SetRecordList() {
            //Hide all options
            lstRecords.addClass('hide');

            //Show records
            if (categoryId != "") {
                //Show records that match category
                $(lstRecords).each(function () {
                    if ($(this).data('categoryid') == categoryId) {
                        $(this).removeClass('hide');
                    }
                });
            }
            else if (toolsetId != "") {
                //Show records that match toolset
                $(lstRecords).each(function () {
                    if ($(this).data('toolid') == toolsetId) {
                        $(this).removeClass('hide');
                    }
                });
            }
        }


        //// Instantiate variables
        //var btnBag = $("#BagList .bag-btn");
        //var btnCreateNewBag = $('#btn-create-new-bag');
        //var btnSubmitNewBag = $('#btn-submit-new-bag');
        //var btnCancelNewBag = $('#btn-cancel-new-bag');
        //var pnlNewBag = $('#pnl-new-bag');

        ////Methods
        //$(btnBag).click(function (e) {
        //    //Set Active button
        //    btnBag.removeClass('active');
        //    $(this).addClass('active');
        //});
        //$(btnCreateNewBag).click(function (e) {
        //    //
        //    pnlNewBag.show();
        //})
        //$(btnSubmitNewBag).click(function (e) {
        //    //
        //    pnlNewBag.hide();
        //})
        //$(btnCancelNewBag).click(function (e) {
        //    //
        //    pnlNewBag.hide();
        //})

        ////Initiallize default settings
        //btnBag.eq(0).addClass('active');
    };
    function jsMemberList() {
        // Instantiate variables
        var btnMember = $("#MemberList .member-btn");

        //Methods
        $(btnMember).click(function (e) {
            //Set Active button
            btnMember.removeClass('active');
            $(this).addClass('active');
        });

        //Initiallize default settings
        btnMember.eq(0).addClass('active');
    };
    function jsCommunications() {

    };


    try {
        //Run common tasks
        jsCommon();

        //Run only if element exists
        if ($('.tab-buttons').length > 0) { jsTabs(); }
        if ($('#MemberList').length > 0) { jsMemberList(); }
        if ($('#Communication-Inventory').length > 0) { jsCommunications(); }
        if ($('.BugoutBag').length > 0) { jsBugoutBags(); }
        if ($('.barcode-pnl').length > 0) { jsbarcodePnl(); }
        //if ($('#BugoutBags').length > 0) { jsBugoutBags(); }
        //if ($('#AccountSetup').length > 0) { jsAccountSetup(); }
    }
    catch (err) {
        console.log('ERROR: ' + err.message);
    }
});








////Circle graphs
////https://get.foundation/building-blocks/blocks/circle-graph.html
//$("[data-circle-graph]").each(function () {
//    var $graph = $(this),
//        percent = parseInt($graph.data('percent'), 10),
//        deg = 360 * percent / 100;
//    if (percent > 50) {
//        $graph.addClass('gt-50');
//    }
//    $graph.find('.circle-graph-progress-fill').css('transform', 'rotate(' + deg + 'deg)');
//    $graph.find('.circle-graph-percents-number').html(percent + '%');
//});

