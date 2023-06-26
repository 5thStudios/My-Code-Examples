//  INSTANTIATE ZURB FOUNDATION
//========================================================
jQuery(function ($) {
    $(document).foundation();
    window.onresize = function () {
        refreshEqualizers();
    }
});



//  RECAPTCHA & CALLBACK
//========================================================
var renderRecaptcha = function () {
    if ($('#RecaptchaContainer').length > 0) {
        grecaptcha.render('RecaptchaContainer', {
            'sitekey': '6LfCtaokAAAAACi1iDIVPiPZkm4Lm2uBisadRbBL',
            'callback': reCaptchaCallback,
            theme: 'light', //light or dark
            size: 'normal'//normal or compact
        });
    }
};
var reCaptchaCallback = function (response) {
    if (response !== '') {
        $('#RecaptchaContainer').removeClass('alert');
        $('.hfldIsValid').val(true);
    }
};



//  SHOW RATIONALE
//========================================================
jQuery(function ($) {
    try {
        function jsShowRationale() {
            //Instantiate variables
            var showRationaleBtn = $("#RationaleAction");

            $(showRationaleBtn).click(function () {
                $(".rationale").toggle();
                //$("#CorrectAnswer").parent(".form-row").toggleClass("CorrectAnswer");
                //UpdateSelectedAnswer();
            });
        }

        //Run only if element exists
        if ($('.exam').length > 0) { jsShowRationale(); }
    }
    catch (err) {
        console.log('ERROR: [jsShowRationale]');
        console.log(err.message);
    }
});



//  INSTANTIATE ACCORDIONS
//========================================================
jQuery(function ($) {
    //=======================================
    // Set parameters for jquery ui accordion.
    //=======================================
    var $accordions = $(".accordion").accordion({
        collapsible: true,
        heightStyle: "content",
        header: "h3",
        active: false //forces all panels in the same accordion to be closed by default
    }).on('click', function () {
        //Close all other accordions except the active one.
        $accordions.not(this).accordion("option", "active", false);
    });
});




//  GET URL PARAMETER BY QUERY NAME
//========================================================
function GetParameterValues(param) {
    var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}



//  EXAM RESULT TOGGLE
//========================================================
$(document).ready(function () {
    try {
        function jsExamMenuToggle() {
            $("ul.exam-menu li h5").click(function () {
                var that = $(this);
                var results = $(that.parent('li').children('.exam-result'));
                results.toggle('slow');
            });
        }

        //Run only if element exists
        if ($('ul.exam-menu').length > 0) { jsExamMenuToggle(); }
    }
    catch (err) {
        console.log('ERROR: [jsExamMenuToggle]  ');
        console.log(err.message);
    }
});



//  SLICK CAROUSEL
//  https://kenwheeler.github.io/slick/
//========================================================
$(document).ready(function () {
    try {
        function jsSlick() {
            $('.slick-carousel').slick({
                autoplay: true,
                autoplaySpeed: 3000,
                arrows: false,
                dots: false,
                lazyLoad: 'ondemand',
                centerMode: true,
                centerPadding: '100px',
                slidesToShow: 5,
                slidesToScroll: 1,
                responsive: [
                    {
                        breakpoint: 1660, //below this point
                        settings: {
                            slidesToShow: 4,
                            centerMode: true
                        }
                    },
                    {
                        breakpoint: 1440, //below this point
                        settings: {
                            slidesToShow: 3,
                            centerMode: true
                        }
                    },
                    {
                        breakpoint: 1024, //below this point
                        settings: {
                            slidesToShow: 2,
                            centerMode: true
                        }
                    },
                    {
                        breakpoint: 768, //below this point
                        settings: {
                            slidesToShow: 1,
                            centerMode: false,
                            centerPadding: '0',
                            adaptiveHeight: true
                        }
                    }
                ]
            });

            refreshEqualizers();
        }

        //Run only if element exists
        if ($('.slick-carousel').length > 0) { jsSlick(); }
    }
    catch (err) {
        console.log('ERROR: [jsSlick] ');
        console.log(err.message);
    }
});



//  REFRESH EQUALIZERS
//========================================================
function refreshEqualizers() {
    try {
        var lstEqualizers = document.querySelectorAll('[data-equalizer]');
        if ($(lstEqualizers).length > 0) {
            //Redo foundation equalizer 
            Foundation.reInit('equalizer');

            //Added 2nd attempt with a delay in case the 1st attempt fails.
            setTimeout(function () { Foundation.reInit('equalizer'); }, 50);
            setTimeout(function () { Foundation.reInit('equalizer'); }, 100);
        }
    }
    catch (err) {
        console.log('ERROR: [refreshEqualizers]');
        console.log(err.message);
    }
}



//  PURCHASE REGIONS BUTTON CLICK
//========================================================
jQuery(function ($) {
    try {
        function jsPurchaseRegion() {
            //Instantiate variables
            var lstRegionBtns = $('.purchase-regions .region');
            var lstTablePurchases = $('.table-purchases');


            //Refresh pg is width changes.
            var pgWidth = window.outerWidth;
            window.onresize = function () {
                if (pgWidth != window.outerWidth) {
                    location.reload();
                }
            }


            //Handles
            $(lstRegionBtns).click(function () {
                //Adjust size of buttons
                $(lstRegionBtns).removeClass('auto');
                $(lstRegionBtns).addClass('shrink');

                $(this).removeClass('shrink');
                $(this).addClass('auto');

                //Mark selected region as active
                $(lstRegionBtns).removeClass('selected');
                $(this).addClass('selected');

                ///Show active region prices
                lstTablePurchases.hide();
                var region = $(this).data('region');
                $('*[data-region="' + region + '"]').show();

                //Redo foundation equalizer               
                refreshEqualizers();
            });
        }

        //Run only if element exists
        if ($('.purchase-regions').length > 0) { jsPurchaseRegion(); }
    }
    catch (err) {
        console.log('ERROR: [jsPurchaseRegion]  ');
        console.log(err.message);
    }
});



//  CALCULATE COSTS/DISCOUNTS
//========================================================
jQuery(function ($) {
    try {
        function jsCalculateCostAndDiscount() {
            //Instantiate variables
            var lstDiscountCodeDisplay = $('.discountCodeDisplay');
            var lstDiscountDiv = $('.discountCodeDisplay + .discountDiv');
            var lstCouponInputs = $('.discountDiv input[type=text]');
            var lstApplyBtns = $('.discountDiv .apply-btn');
            var lstCodePnls = $('.discountDiv .code')
            var lstPurchasePanel = $('.purchase-panel');
            var lstRadioBtns = $('input[type="radio"]');
            var lstCheckboxBtns = $('input[type="checkbox"]');
            var lstSavingPanels = $('.price .savings');
            var _index;
            var body = $('body');

            //Global hidden fields
            var hfldValidCoupon = $('#hfldValidCoupon');
            var hfldCouponCode = $('.hfldCouponCode');
            var hfldDiscountByPercentage = $('#hfldDiscountByPercentage');
            var hfldDiscountAmount = $('#hfldDiscountAmount');
            var hfldDiscountPercentage = $('#hfldDiscountPercentage');

            var amountSubtotal = 0;

            //var lstInfoPnls = $('.info');
            //var activeDiscountCodeDisplay;
            //var activeApplyBtns;




            //Handles
            $(lstDiscountCodeDisplay).click(function () {
                //Save active discount panel
                activeDiscountCodeDisplay = $(this);

                //Show active panel
                $(lstDiscountDiv).css("opacity", "0");
                $(this).next(".discountDiv").css("opacity", "1");
            });
            $(lstApplyBtns).click(function () {
                //Save active button
                activeApplyBtns = $(this);

                //Obtain entered coupon code
                _index = $(lstApplyBtns).index(this);
                var _inpVal = $(lstCouponInputs).eq(_index).val();

                //Call api
                submitCoupon(_inpVal);

            });
            $(lstRadioBtns).click(function () {
                estimateCosts();
            });
            $(lstCheckboxBtns).click(function () {
                estimateCosts();
            });


            //Methods
            function estimateCosts() {

                //Loop through each purchase panel
                $(lstPurchasePanel).each(function () {
                    if ($(hfldValidCoupon).val() == 'true') {
                        //
                        $('.coupon-applied').show();

                        if ($(this).hasClass('single-item')) {
                            //Update single-item section
                            var _price = $(this).find("input[type='radio']:checked").data('price');
                            amountSubtotal = _price;

                            //Instantiate variables
                            var _percentage = 0;
                            var _couponSavings = 0;
                            var _origSavings = Number($(this).find('.hfldBundleDiscount').val());
                            var _total = 0;

                            //Get discount
                            if (hfldDiscountByPercentage.val() == 'true') {
                                //Calculate savings
                                _percentage = $(hfldDiscountPercentage).val();
                                _couponSavings = _percentage * _price;
                                _total = (_price - _origSavings - (_percentage * _price)).toFixed(2);

                                //Update hidden fields by percentage
                                $(this).find('.hfldCouponDiscount').val(_couponSavings);
                                $(this).find('.hfldTotalDiscount').val(Number(_origSavings) + Number(_couponSavings));
                                //Set minumum payment
                                if (_total < .5) {
                                    var _diff = _total;
                                    _total = .50;
                                    //$(this).find('.hfldTotalDiscount').val((Number($(this).find('.hfldTotalDiscount').val())) - _diff);
                                }
                                $(this).find('.hfldTotalCost').val(_total);

                                //$(this).find('.amount').text(_total.toFixed(2));
                                //$(this).find('.amount').text(Number(_total).toFixed(2).replace(/\.0+$/, ''));  //remove .00 if applicable

                                //Show final price
                                if (Number.isInteger(_total)) {
                                    $(this).find('.amount').text(_price - _couponSavings);
                                }
                                else {
                                    //$(this).find('.amount').text(_total.toFixed(2));
                                    $(this).find('.amount').text(Number(_total).toFixed(2).replace(/\.0+$/, ''));  //remove .00 if applicable
                                }
                            }
                            else {
                                //Calculate savings
                                _couponSavings = Number($(hfldDiscountAmount).val());
                                _total = (_price - _couponSavings - _origSavings).toFixed(2);

                                //Update hidden fields by whole number
                                $(this).find('.hfldCouponDiscount').val(_couponSavings);
                                $(this).find('.hfldTotalDiscount').val(Number(_origSavings) + Number(_couponSavings));
                                //Set minumum payment
                                if (_total < .5) {
                                    var _diff = _total;
                                    _total = .50;
                                    //$(this).find('.hfldTotalDiscount').val((Number($(this).find('.hfldTotalDiscount').val())) - _diff);
                                }
                                $(this).find('.hfldTotalCost').val(Number(_total));
                                $(this).find('.amount').text(Number(_total).toFixed(2).replace(/\.0+$/, ''));  //remove .00 if applicable

                                //if ($())

                            }


                        }
                        else if ($(this).hasClass('custom-bundle')) {
                            //Instantiate variables
                            var _price = 0;
                            //var _origSavings = Number($(this).find('.hfldBundleDiscount').val());
                            var _discPercentage = $(hfldDiscountPercentage).val();
                            var _discAmount = $(hfldDiscountAmount).val();
                            var _couponSavings = 0;
                            var _total = 0;

                            //Calculate price by adding checked items.
                            var _cbList = $(this).find("input[type='checkbox']:checked");
                            $(_cbList).each(function () {
                                _price = _price + $(this).data('price');
                            });
                            var discountedPrice = calculateDiscounted(_price);


                            //Get discount
                            if (hfldDiscountByPercentage.val() == 'true') {
                                //Calculate coupon savings
                                _couponSavings = discountedPrice * _discPercentage;

                                //Calculate total after savings
                                _total = discountedPrice - _couponSavings;

                                //Update hidden fields
                                amountSubtotal = _price; //$(this).find('.amount-subtotal').val(_price);
                                $(this).find('.hfldBundleDiscount').val(_price - discountedPrice);
                                $(this).find('.hfldCouponDiscount').val(_couponSavings);
                                $(this).find('.hfldTotalDiscount').val(Number(amountSubtotal) - Number(_total));
                                //Set minumum payment
                                if (_total < .5) {
                                    var _diff = _total;
                                    _total = .50;
                                    //$(this).find('.hfldTotalDiscount').val((Number($(this).find('.hfldTotalDiscount').val())) - _diff);
                                }
                                $(this).find('.hfldTotalCost').val(_total);

                                //Show result total
                                if (Number.isInteger(_total)) {
                                    $(this).find('.amount').text(_total);
                                }
                                else {
                                    //$(this).find('.amount').text(_total.toFixed(2));
                                    $(this).find('.amount').text(Number(_total).toFixed(2).replace(/\.0+$/, ''));  //remove .00 if applicable
                                }
                            }
                            else {
                                //Calculate total after savings
                                _total = discountedPrice - _discAmount;

                                //Update hidden fields
                                amountSubtotal = _price; //$(this).find('.amount-subtotal').val(_price);
                                $(this).find('.hfldBundleDiscount').val(_price - discountedPrice);
                                $(this).find('.hfldCouponDiscount').val(_discAmount);
                                $(this).find('.hfldTotalDiscount').val(Number(amountSubtotal) - Number(_total));
                                //Set minumum payment
                                if (_total < .5) {
                                    var _diff = _total;
                                    _total = .50;
                                    //$(this).find('.hfldTotalDiscount').val((Number($(this).find('.hfldTotalDiscount').val())) - _diff);
                                }
                                $(this).find('.hfldTotalCost').val(_total);

                                //Show result total
                                if (Number.isInteger(_total)) {
                                    $(this).find('.amount').text(_total);
                                }
                                else {
                                    //$(this).find('.amount').text(_total.toFixed(2));
                                    $(this).find('.amount').text(Number(_total).toFixed(2).replace(/\.0+$/, ''));  //remove .00 if applicable
                                }
                            }
                        }
                        else if ($(this).hasClass('complete-bundle')) {

                            //Instantiate variables
                            var _price = 0;
                            var _discPercentage = Number($(hfldDiscountPercentage).val());
                            var _discAmount = Number($(hfldDiscountAmount).val());
                            var _couponSavings = 0;
                            var _total = 0;


                            //Calculate price by adding checked items.
                            var _cbList = $(this).find("p.item");
                            $(_cbList).each(function () {
                                _price = _price + Number($(this).data('price'));
                            });

                            //var discountedPrice = calculateDiscounted(_price);
                            var bundleDiscount = Number($(this).find('.hfldBundleDiscount').val());
                            var discountedPrice = (_price - bundleDiscount);


                            //Get discount
                            if (hfldDiscountByPercentage.val() == 'true') {
                                //Calculate coupon savings
                                _couponSavings = discountedPrice * _discPercentage;

                                //Calculate total after savings
                                _total = discountedPrice - _couponSavings;

                                //Update hidden fields
                                amountSubtotal = _price; //$(this).find('.amount-subtotal').val(_price);
                                //$(this).find('.hfldBundleDiscount').val(_price - discountedPrice);
                                $(this).find('.hfldCouponDiscount').val(_couponSavings);
                                $(this).find('.hfldTotalDiscount').val(amountSubtotal - _total);
                                //Set minumum payment
                                if (_total < .5) {
                                    var _diff = _total;
                                    _total = .50;
                                    //$(this).find('.hfldTotalDiscount').val((Number($(this).find('.hfldTotalDiscount').val())) - _diff);
                                }
                                $(this).find('.hfldTotalCost').val(_total);

                                //Show result total
                                if (Number.isInteger(_total)) {
                                    $(this).find('.amount').text(_total);
                                }
                                else {
                                    //$(this).find('.amount').text(_total.toFixed(2));
                                    $(this).find('.amount').text(Number(_total).toFixed(2).replace(/\.0+$/, ''));  //remove .00 if applicable
                                }
                            }
                            else {
                                //Calculate total after savings
                                _total = discountedPrice - _discAmount;

                                //Update hidden fields
                                amountSubtotal = _price; //$(this).find('.amount-subtotal').val(_price);
                                //$(this).find('.hfldBundleDiscount').val(_price - discountedPrice);
                                $(this).find('.hfldCouponDiscount').val(_discAmount);
                                $(this).find('.hfldTotalDiscount').val(Number(amountSubtotal) - Number(_total));
                                //Set minumum payment
                                if (_total < .5) {
                                    var _diff = _total;
                                    _total = .50;
                                    //$(this).find('.hfldTotalDiscount').val((Number($(this).find('.hfldTotalDiscount').val())) - _diff);
                                }
                                $(this).find('.hfldTotalCost').val(_total);

                                //Show result total
                                if (Number.isInteger(_total)) {
                                    $(this).find('.amount').text(_total);
                                }
                                else {
                                    //$(this).find('.amount').text(_total.toFixed(2));
                                    $(this).find('.amount').text(Number(_total).toFixed(2).replace(/\.0+$/, ''));  //remove .00 if applicable
                                }
                            }
                        }

                        //Show discount amount
                        var _discount = Number($(this).find('.hfldBundleDiscount').val()) + Number($(this).find('.hfldCouponDiscount').val());
                        if (Number.isInteger(_discount)) {
                            $(this).find('.price .discount').text(_discount);
                        }
                        else {
                            $(this).find('.price .discount').text(Number(_discount).toFixed(2));
                        }

                        $(this).find('.price .savings').show();

                    }
                    else {
                        //Retail price only.
                        if ($(this).hasClass('single-item')) {
                            //Update single-item section
                            var _price = $(this).find("input[type='radio']:checked").data('price');

                            //Update hidden fields
                            amountSubtotal = _price; //$(this).find('.amount-subtotal').val(_price);
                            $(this).find('.hfldBundleDiscount').val(0);
                            $(this).find('.hfldTotalDiscount').val(0);
                            $(this).find('.hfldTotalCost').val(_price);
                            $(this).find('.amount').text(_price);
                        }
                        else if ($(this).hasClass('custom-bundle')) {
                            //
                            var _price = 0;
                            var _cbList = $(this).find("input[type='checkbox']:checked");
                            $(_cbList).each(function () {
                                _price = _price + Number($(this).data('price'));
                            });

                            var discountedPrice = calculateDiscounted(_price);

                            //Update hidden fields
                            amountSubtotal = _price; //$(this).find('.amount-subtotal').val(_price);
                            $(this).find('.hfldBundleDiscount').val(_price - discountedPrice);
                            $(this).find('.hfldTotalDiscount').val(_price - discountedPrice);
                            $(this).find('.hfldTotalCost').val(discountedPrice);

                            $(this).find('.amount').text(discountedPrice);

                            if (Number.isInteger(_price - discountedPrice)) {
                                //Show whole number
                                $(this).find('.price .discount').text(_price - discountedPrice);
                            }
                            else {
                                //Show with decimal
                                $(this).find('.price .discount').text(Number(_price - discountedPrice).toFixed(2));
                            }

                        }
                        else if ($(this).hasClass('complete-bundle')) {
                            //
                            var _price = 0;
                            var _cbList = $(this).find("p.item");
                            $(_cbList).each(function () {
                                _price = _price + Number($(this).data('price'));
                            });

                            //var discountedPrice = calculateDiscounted(_price);
                            var bundleDiscount = Number($(this).find('.hfldBundleDiscount').val());
                            var discountedPrice = (_price - bundleDiscount);

                            //Update hidden fields
                            //amountSubtotal = _price; //$(this).find('.amount-subtotal').val(_price);
                            //$(this).find('.hfldBundleDiscount').val(_price - discountedPrice);
                            $(this).find('.hfldTotalDiscount').val(bundleDiscount);
                            $(this).find('.hfldTotalCost').val(discountedPrice);

                            $(this).find('.amount').text(discountedPrice);

                            if (Number.isInteger(_price - discountedPrice)) {
                                $(this).find('.price .discount').text(_price - discountedPrice);
                            }
                            else {
                                $(this).find('.price .discount').text(Number(_price - discountedPrice).toFixed(2));
                            }
                        }
                    }
                });

                //Update hidden fields to mark what is selected.
                updateSelectedHflds();
            }
            function updateSelectedHflds() {
                //Loop through each purchase panel
                $(lstPurchasePanel).each(function () {
                    if ($(this).hasClass('single-item')) {
                        //Update single-item section
                        $(this).find("input[type='hidden'].hfldIsSelected").val(false);

                        //
                        var _id = $(this).find("input[type='radio']:checked").attr('id');
                        $(this).find(".hflds." + _id + " .hfldIsSelected").val(true);
                    }
                    else if ($(this).hasClass('custom-bundle')) {
                        //Update custom-bundle section
                        $(this).find("input[type='hidden'].hfldIsSelected").val(false);

                        //
                        var _this = $(this);
                        var _cbList = $(this).find("input[type='checkbox']:checked");
                        $(_cbList).each(function () {
                            var _id = $(this).attr('id');
                            $(_this).find(".hflds." + _id + " .hfldIsSelected").val(true);
                        });

                    }
                    else if ($(this).hasClass('complete-bundle')) {
                        //
                        //var _price = $(this).find(".hfldTotalCost").val();
                        //$(this).find('.amount').text(_price);
                    }

                });
            }
            function submitCoupon(coupon) {
                //Valid email address. Instantiate variables
                var urlPath = window.location.protocol + '//' + window.location.host;
                var internalApiUrl = urlPath + '/umbraco/Api/SwtpApi/ValidateCoupon?coupon=' + coupon;


                //Call AJAX service
                var response = CallService_POST();
                var promise = $.when(response);
                promise.then(function () { ServiceSucceeded(response); });
                promise.done(function () { refreshEqualizers(); });
                promise.fail(function () { ServiceFailed(response); });


                //METHODS
                function CallService_POST() {
                    //Force cursor to show that the coupon search is in progress
                    $(body).addClass('pending');

                    try {
                        return $.ajax({
                            url: internalApiUrl,
                            type: "POST",
                            data: null,
                            dataType: "json",
                            contentType: false,
                            processData: false
                        });

                    }
                    catch (err) {
                        if (err.message !== null) {
                            console.log('AJAX ERROR: ' + err.message);
                        }
                    }
                }
                function ServiceFailed(result) {
                    //Remove cursor override
                    $(body).removeClass('pending');

                    //clear data
                    Type = null;
                    varUrl = null;
                    Data = null;
                    ContentType = null;
                    DataType = null;
                    processData = null;

                    //Show message
                    $(lstCodePnls).eq(_index).find('.info').hide();
                    $(lstCodePnls).eq(_index).find('.info.invalid').show();
                }
                function ServiceSucceeded(searchResults) {
                    //Remove cursor override
                    $(body).removeClass('pending');

                    //Convert result into object
                    var response = JSON.parse(searchResults.responseJSON);

                    //Determine how to show response
                    if (response.IsValid) {
                        SetAsValidSubmission(response);
                    }
                    else {
                        SetAsInvalidSubmission(response.Msg);
                    }
                }
                function SetAsValidSubmission(response) {
                    //Update according to valid return values
                    $(hfldValidCoupon).val(true);
                    $(hfldCouponCode).val(response.Coupon.Code);

                    //Clear fields
                    $(hfldDiscountAmount).val('');
                    $(hfldDiscountPercentage).val('');

                    //Store discount amount ($ or %)
                    $(hfldDiscountByPercentage).val(response.Coupon.DiscountByPercentage);
                    if (response.Coupon.DiscountByPercentage == true) {
                        $(hfldDiscountPercentage).val(response.Coupon.DiscountPercent * .01);
                        //Set discount amount msg.
                        $(lstCodePnls).eq(_index).find('.info.valid .discount-value').text(response.Coupon.DiscountPercent + '%');
                    }
                    else {
                        $(hfldDiscountAmount).val(response.Coupon.DiscountAmount);
                        //Set discount amount msg.
                        $(lstCodePnls).eq(_index).find('.info.valid .discount-value').text('$' + response.Coupon.DiscountAmount);
                    }

                    //Show message
                    $(lstCodePnls).eq(_index).find('.info').hide();
                    $(lstCodePnls).eq(_index).find('.info.valid').show();

                    //Refresh costs
                    estimateCosts();
                }
                function SetAsInvalidSubmission(errorMsg) {
                    //Show message
                    $(lstCodePnls).eq(_index).find('.info').hide();
                    $(lstCodePnls).eq(_index).find('.info.invalid').show();
                    $(lstSavingPanels).hide();

                    $(lstCodePnls).addClass('alert');
                    setTimeout(function () {
                        $(lstCodePnls).removeClass('alert');
                    }, 2000)

                    //Refresh costs
                    estimateCosts();
                }
            }
            function calculateDiscounted(price) {
                //if (price < 40.01) return calculateCouponDiscount(price);
                if (price < 40.01) return price;

                var rawDiscountPercenter = (price - 40) * 0.0015 + 0.02;
                var rawDiscount = rawDiscountPercenter * price;
                var rawPrice = price - rawDiscount;

                var discountedPrice = Math.round(rawPrice / 5) * 5 - 1;

                return discountedPrice;
                //return calculateCouponDiscount(bundleDiscountedPrice);
            };


            //Refresh costs
            estimateCosts();
        };


        //Run only if element exists
        if ($('.purchase-regions').length > 0) { jsCalculateCostAndDiscount(); }
    }
    catch (err) {
        console.log('ERROR: [jsCalculateCostAndDiscount] ');
        console.log(err.message);
    }
});



//  UPDATE TIMEZONE-RELATED DATE FIELDS
//========================================================
jQuery(function ($) {
    function AdjustDateTimes() {
        //Create date/time settings
        var optionDate = { month: 'short', day: 'numeric', year: 'numeric' };
        var optionTime = { hour: 'numeric', minute: '2-digit' };

        //Instantiate variables
        var lstRecords = $("[data-datetime]");

        // NOTE:  This time adjustment will likely be wrong when testing on my local database.  Is correct when tested on live database in GB
        lstRecords.each(function () {
            //Get date from span
            var strDateTime = $(this).data('datetime');

            if (strDateTime.length > 0) {
                //Convert string into date and adjust for timezone
                var date = new Date(strDateTime);
                var offset = new Date().getTimezoneOffset();
                date.setMinutes(date.getMinutes() - offset);
                //Display adjusted date/time
                $(this).html('- ' + date.toLocaleString("en-US", optionDate) + " @ " + date.toLocaleString("en-US", optionTime));
            }
        });
    }

    //Run only if element exists
    if ($("[data-datetime]").length > 0) { AdjustDateTimes(); }
});
