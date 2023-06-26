angular
    .module('umbraco')
    .controller('manageCouponsController', function ($scope) {

        //  INSTANTIATE VARIABLES
        /* buttons */
        var btnCouponLookup = $('#btnCouponLookup');
        var btnSaveCode = $('#btnSaveCode');

        /* text boxes & fields */
        var txbCouponCode = $('#txbCouponCode');
        var txbDiscount = $('#txbDiscount');
        var ddlDiscountType = $('#ddlDiscountType');
        var txbDatePicker = $('#txbDatePicker');
        var txbMaxAllowed = $('#txbMaxAllowed');
        var lblTimesUsed = $('#lblTimesUsed');
        var txbNotes = $('#txbNotes');
        var lblCreatedDate = $('#lblCreatedDate');
        var hfldJson = $('.manageCouponsController #hfldJson');

        /* msgs & panels */
        var tblMngCouponsBody = $('.tblMngCoupons > tbody');
        var msgPercentInvalid = $('.msgPercentInvalid');
        var msgDiscountRequired = $('.msgDiscountRequired');
        var msgDiscountPositiveNo = $('.msgDiscountPositiveNo');
        var msgInvalidDate = $('.msgInvalidDate');
        var msgInvalidTimesUsed = $('.msgInvalidTimesUsed');
        var msgErrorSaving = $('.msgErrorSaving');
        var savedSuccessfully = $('.savedSuccessfully');

        /* vars  */
        var body = $('body');
        var urlPath = window.location.protocol + '//' + window.location.host;
        var internalApi = "";

        //Hide msgs
        savedSuccessfully.hide();
        msgErrorSaving.hide();
        msgPercentInvalid.hide();
        msgDiscountRequired.hide();
        msgDiscountPositiveNo.hide();



        //Handles
        $(btnCouponLookup).click(function (e) {
            //Hide msgs
            savedSuccessfully.hide();
            msgErrorSaving.hide();

            //Proceed only if value has been entered
            if (txbCouponCode.val()) {
                //Force cursor to show that the coupon search is in progress
                $(body).addClass('pending');

                //Create api call with data
                internalApi = urlPath + '/umbraco/Api/SwtpApi/ValidateCoupon?coupon=' + txbCouponCode.val();


                //Call AJAX service
                var response = CallService_POST();
                var promise = $.when(response);
                promise.done(function () { ServiceSucceeded(response); });
                promise.fail(function () { ServiceFailed(response); });


                //METHODS
                function CallService_POST() {
                    try {
                        return $.ajax({
                            url: internalApi,
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
                    console.log('Service call failed: ' + result.status + ' | ' + result.statusText + ' | ' + result.responseText);

                    //clear data
                    Type = null;
                    varUrl = null;
                    Data = null;
                    ContentType = null;
                    DataType = null;
                    processData = null;

                    //Error message
                    console.log('Error Msg:');
                    console.log(result);

                    //Show message
                    alert("ERROR: " + result);


                    //Remove cursor override
                    $(body).removeClass('pending');
                }
                function ServiceSucceeded(searchResults) {
                    var jsonData = JSON.parse(searchResults.responseJSON);
                    console.log(jsonData);

                    //Show settings
                    tblMngCouponsBody.show();

                    //clear all fields
                    txbDiscount.val('');
                    ddlDiscountType.val('Amount');
                    txbDatePicker.val('');
                    txbMaxAllowed.val('');
                    lblTimesUsed.html(' / 0');
                    txbNotes.val('');
                    lblCreatedDate.html('');




                    //Populate fields if data returned
                    if (jsonData.IsValid) {
                        if (jsonData.Coupon.DiscountByPercentage) {
                            txbDiscount.val(jsonData.Coupon.DiscountPercent);
                            ddlDiscountType.val('Percent');
                        }
                        else {
                            txbDiscount.val(jsonData.Coupon.DiscountAmount);
                            ddlDiscountType.val('Amount');
                        }
                        if (jsonData.Coupon.ExpireDate) {
                            //Convert expiration date to proper format [2023-06-21]
                            var d = new Date(jsonData.Coupon.ExpireDate);
                            var dd = d.getDate();
                            var mm = d.getMonth() + 1;
                            var yy = d.getFullYear();
                            if (dd < 10) dd = "0" + dd;
                            if (mm < 10) mm = "0" + mm;
                            txbDatePicker.val(yy + '-' + mm + '-' + dd);
                            //console.log(yy + '-' + mm + '-' + dd);
                            //console.log(new Date(jsonData.Coupon.ExpireDate).toLocaleDateString("en-US", { year: 'numeric', month: '2-digit', day: '2-digit' }));
                        }
                        if (jsonData.Coupon.TimesUsedLimit) { txbMaxAllowed.val(jsonData.Coupon.TimesUsedLimit); }
                        if (jsonData.Coupon.TimesUsed) { lblTimesUsed.html(' / ' + jsonData.Coupon.TimesUsed); }
                        else {

                        }
                        if (jsonData.Coupon.Notes) { txbNotes.val(jsonData.Coupon.Notes); }
                        lblCreatedDate.html(new Date(jsonData.Coupon.CreateDate).toLocaleDateString("en-US", { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }));
                    }

                    //Set required fields
                    $(txbDiscount).prop('required', true);
                    txbDiscount.prop('required', true);

                    //Remove cursor override
                    $(body).removeClass('pending');
                }


                //Remove cursor override
                $(body).removeClass('pending');

            }

        });
        $(btnSaveCode).click(function (e) {
            //Validate fields
            if (txbDiscount.val().length <= 0) {
                msgDiscountRequired.show();
            }
            else if ((ddlDiscountType.val() == 'Percent') && (txbDiscount.val() < 0 || txbDiscount.val() > 100)) {
                msgPercentInvalid.show();
            }
            else if (txbDiscount.val() < 0) {
                msgDiscountPositiveNo.show();
            }
            //Proceed only if value has been entered
            if (txbCouponCode.val()) {

                //Force cursor to show that the coupon search is in progress
                $(body).addClass('pending');

                console.log('btnSaveCode clicked');

                //
                var data = CreateParameters();

                //Create api call with data
                internalApi = urlPath + '/umbraco/Api/SwtpApi/UpdateCoupon?data=' + data; // + txbCouponCode.val();


                //Call AJAX service
                var response = CallService_POST();
                var promise = $.when(response);
                promise.done(function () { ServiceSucceeded(response); });
                promise.fail(function () { ServiceFailed(response); });


                //METHODS
                function CreateParameters() {
                    //Instantiate an array of parameters to pass to handler
                    var myData = {};
                    myData.CouponName = txbCouponCode.val();
                    myData.DiscountAmount = txbDiscount.val();
                    myData.DiscountType = ddlDiscountType.val();
                    myData.Expires = txbDatePicker.val();
                    myData.MaxAllowed = txbMaxAllowed.val();
                    myData.Notes = txbNotes.val();
                    //Set array as json for use in ajax call
                    return JSON.stringify(myData);
                }
                function CallService_POST() {
                    try {
                        return $.ajax({
                            url: internalApi,
                            type: "POST",
                            data: null,
                            dataType: "json",
                            contentType: false,
                            processData: false

                            //data: data,
                            //dataType: "json",
                            //contentType: "application/json; charset=utf-8",
                            //processData: true
                        });

                    }
                    catch (err) {
                        if (err.message !== null) {
                            console.log('AJAX ERROR: ' + err.message);
                        }
                        else {
                            console.log('AJAX ERROR');
                        }
                    }
                }
                function ServiceFailed(result) {
                    console.log('Service call failed: ' + result.status + ' | ' + result.statusText + ' | ' + result.responseText);

                    //clear data
                    Type = null;
                    varUrl = null;
                    Data = null;
                    ContentType = null;
                    DataType = null;
                    processData = null;

                    //Error message
                    console.log('Error Msg:');
                    console.log(result);

                    //Show message
                    alert("ERROR: " + result);
                    msgErrorSaving.show();

                    //Remove cursor override
                    $(body).removeClass('pending');
                }
                function ServiceSucceeded(searchResults) {
                    console.log('Service Succeeded');
                    var jsonData = JSON.parse(searchResults.responseJSON);
                    console.log(jsonData);

                    //Show msg
                    if (searchResults.responseJSON.indexOf('Error:') != -1) {
                        msgErrorSaving.show();
                    }
                    else {
                        savedSuccessfully.show();
                    }

                    //Remove cursor override
                    $(body).removeClass('pending');
                }


                //Remove cursor override
                $(body).removeClass('pending');

            }

        });

    });


//  Manage Coupons
//  ====================
//  Lookup 'click':
//  	if field[codeName] is blank
//  		show error
//  	else
//  		If coupon exists
//  			Pull data
//  			populate fields
//  		else
//  			leave fields blank

//  		Show "Coupon Settings"



//  Save Changes 'click':
//  	Validate fields:
//  		discount
//  			not empty and is numeric
//  			if $
//  				must be greater than 0
//  			else if %
//  				must be between 0-100
//  		expire date is valid date IF populated.  (not required)
//  		max allowed is numeric IF populate.  (not required)

//  	if valid
//  		if exists
//  			update
//  		else
//  			create new
