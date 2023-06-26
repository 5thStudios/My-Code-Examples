angular
    .module('umbraco')
    .controller('subscriptionsController', function ($scope) {

        //  INSTANTIATE VARIABLES
        /* buttons */
        var btnMemberSearch = $('#btnMemberSearch');
        var btnGiveBonusButton = $('#btnGiveBonusButton');
        var btnExtendSelected = $('#btnExtendSelected');
        var btnSelectAll = $('#btnSelectAll');

        /* text boxes & fields */
        var txbMemberEmail = $('#txbMemberEmail');
        var ddlGiveExamSelect = $('#ddlGiveExamSelect');
        var txbDays = $('#txbDays');
        var tblExamRecordList = $('#examRecordList');
        var examRecordCbList; // = $('#examRecordList .cbExamRecord');
        //var txbAttempts = $('#txbAttempts');

        /* msgs & panels */
        var tblSettings = $('.tblSubscriptions > tbody');
        var tblSubscriptions = $('.tblSubscriptions > tfoot');
        var msgMemberDoesNotExist = $('.msgMemberDoesNotExist');
        var msgEnterValue = $('.msgEnterValue');
        var msgSelectRecord = $('.msgSelectRecord');
        var msgErrorSaving = $('.msgErrorSaving');
        var msgSavedSuccessfully = $('.savedSuccessfully');

        /* vars  */
        var jsonData;
        var body = $('body');
        var urlPath = window.location.protocol + '//' + window.location.host;
        var internalApi = "";
        const months = ["Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sept", "Oct", "Nov", "Dec"];
        //var examRecordList = [];


        //Hide msgs and panels
        hideMsgs();
        tblSettings.hide();
        tblSubscriptions.hide();





        //Handles
        $(btnMemberSearch).click(function (e) {
            //Hide msgs
            hideMsgs();

            //Proceed only if value has been entered
            if (txbMemberEmail.val()) {
                //Force cursor to show that the coupon search is in progress
                $(body).addClass('pending');


                //Create api call with data
                internalApi = urlPath + '/umbraco/Api/SwtpApi/Subscriptions_SearchMember?data=' + encodeURIComponent(txbMemberEmail.val());


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
                    //console.log('Service call failed: ' + result.status + ' | ' + result.statusText + ' | ' + result.responseText);

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
                    hideMsgs();
                    tblSettings.hide();
                    tblSubscriptions.hide();
                    msgMemberDoesNotExist.show();

                    //Remove cursor override
                    $(body).removeClass('pending');
                }
                function ServiceSucceeded(searchResults) {
                    //Parse incoming data
                    jsonData = JSON.parse(searchResults.responseJSON);
                    console.log(jsonData);

                    //clear all fields
                    clearAllFields();

                    //Populate fields if data returned
                    if (jsonData.IsValidMember) {
                        //Show settings & fields
                        tblSettings.show();
                        tblSubscriptions.show();

                        //Add each bonus exam record to ddl
                        var isFirst = true;
                        $.each(jsonData.LstBonusExams, function () {
                            ddlGiveExamSelect.append(new Option(this.Title, this.Id, isFirst, isFirst));
                            isFirst = false;
                        });

                        //Add new records.
                        $.each(jsonData.LstExamSubscription, function () {
                            //Convert expiration date to proper format [Mar 23, 2023]
                            var d = new Date(this.ExpirationDate);
                            var dd = d.getDate();
                            var mm = months[d.getMonth()];
                            var yy = d.getFullYear();
                            var expirationDate = mm + ' ' + dd + ', ' + yy;

                            //Add tr to table
                            var opening = '<tr class="text-left examRecord">';
                            var tdCb = '<td><input type="checkbox" class="cbExamRecord" value="' + this.Id + '" /></td>';
                            var tdlExam = '<td><label>' + this.Title + '</label></td>';
                            var tdExpires = '<td><label>' + expirationDate + '</label></td>';
                            var lblStatus = '<td><label>' + this.Status + '</label></td>';
                            var closing = '</tr>';
                            tblExamRecordList.append(opening + tdCb + tdlExam + tdExpires + lblStatus + closing);
                        });

                    }
                    else {
                        //hide settings & fields
                        tblSettings.hide();
                        tblSubscriptions.hide();
                    }

                    //Remove cursor override
                    $(body).removeClass('pending');
                }


                //Remove cursor override
                $(body).removeClass('pending');
            }

        });
        $(btnGiveBonusButton).click(function (e) {
            //Hide msgs
            hideMsgs();


            //Create data to pass to api
            var data = CreateParameters();
            function CreateParameters() {
                //Obtain all current checkboxes
                examRecordCbList = null;
                examRecordCbList = $('#examRecordList input.cbExamRecord:checked');


                //Update json data with updated info
                jsonData.BonusExamId = ddlGiveExamSelect.val();


                //Convert json to string for api submission
                return JSON.stringify(jsonData);
            }


            //Proceed if fields are valid
            //Force cursor to show that the coupon search is in progress
            $(body).addClass('pending');


            //Create api call with data
            internalApi = urlPath + '/umbraco/Api/SwtpApi/Subscriptions_GiveExamToMember'; //?data=' + encodeURIComponent(data);


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
                        data: data,
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
                jsonData = JSON.parse(searchResults.responseJSON);
                console.log(jsonData);


                //Show msg
                msgSavedSuccessfully.show();


                //Add new records.
                tblExamRecordList.html('');
                $.each(jsonData.LstExamSubscription, function () {
                    //Convert expiration date to proper format [Mar 23, 2023]
                    var d = new Date(this.ExpirationDate);
                    var dd = d.getDate();
                    var mm = months[d.getMonth()];
                    var yy = d.getFullYear();
                    var expirationDate = mm + ' ' + dd + ', ' + yy;

                    //Add tr to table
                    var opening = '<tr class="text-left examRecord">';
                    var tdCb = '<td><input type="checkbox" class="cbExamRecord" value="' + this.Id + '" /></td>';
                    var tdlExam = '<td><label>' + this.Title + '</label></td>';
                    var tdExpires = '<td><label>' + expirationDate + '</label></td>';
                    var lblStatus = '<td><label>' + this.Status + '</label></td>';
                    var closing = '</tr>';
                    tblExamRecordList.append(opening + tdCb + tdlExam + tdExpires + lblStatus + closing);
                });


                //Remove cursor override
                $(body).removeClass('pending');
            }
        });
        $(btnExtendSelected).click(function (e) {
            //Hide msgs
            hideMsgs();

            //Create data to pass to api
            var cbCount = 0;
            var data = CreateParameters();
            function CreateParameters() {
                //Obtain all current checkboxes
                examRecordCbList = null;
                examRecordCbList = $('#examRecordList input.cbExamRecord:checked');


                //Add extend days
                jsonData.ExtendDays = txbDays.val();


                //Clear each cb select value in json and select checked ones.
                $.each(jsonData.LstExamSubscription, function (i, record) {
                    record.IsSelected = false;
                    $.each(examRecordCbList, function (ii, cb) {
                        if (cb.value == record.Id) {
                            record.IsSelected = true;
                            cbCount++;
                        }
                    });
                });

                //Convert json to string for api submission
                return JSON.stringify(jsonData);
            }


            //Proceed if fields are valid
            if (cbCount == 0) {
                msgSelectRecord.show();
            }
            else if (!txbDays.val()) {
                msgEnterValue.show();
            }
            else {
                //Force cursor to show that the coupon search is in progress
                $(body).addClass('pending');


                //Create api call with data
                internalApi = urlPath + '/umbraco/Api/SwtpApi/Subscriptions_ExtendSelectedSubscriptions';

                //TEST
                //internalApi = urlPath + '/umbraco/Api/SwtpApi/ApiTestWithDatastream';


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
                            data: data, //encodeURIComponent(data),
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

                    //console.log(' ');
                    //console.log('ServiceSucceeded');
                    //console.log(searchResults.responseJSON);
                    //console.log(' ');


                    jsonData = JSON.parse(searchResults.responseJSON);
                    console.log(jsonData);


                    //Show msg
                    msgSavedSuccessfully.show();


                    //Add new records.
                    tblExamRecordList.html('');
                    $.each(jsonData.LstExamSubscription, function () {
                        //Convert expiration date to proper format [Mar 23, 2023]
                        var d = new Date(this.ExpirationDate);
                        var dd = d.getDate();
                        var mm = months[d.getMonth()];
                        var yy = d.getFullYear();
                        var expirationDate = mm + ' ' + dd + ', ' + yy;

                        //Add tr to table
                        var opening = '<tr class="text-left examRecord">';
                        var tdCb = '<td><input type="checkbox" class="cbExamRecord" value="' + this.Id + '" /></td>';
                        var tdlExam = '<td><label>' + this.Title + '</label></td>';
                        var tdExpires = '<td><label>' + expirationDate + '</label></td>';
                        var lblStatus = '<td><label>' + this.Status + '</label></td>';
                        var closing = '</tr>';
                        tblExamRecordList.append(opening + tdCb + tdlExam + tdExpires + lblStatus + closing);
                    });


                    //Remove cursor override
                    $(body).removeClass('pending');
                }


                //Remove cursor override
                $(body).removeClass('pending');
            }
        });
        $(btnSelectAll).click(function (e) {
            //
            var cbList = $('#examRecordList input.cbExamRecord');
            $.each(cbList, function () {
                $(this).prop('checked', true);
            });
        })



        //Methods
        function hideMsgs() {
            //Hide all messages
            msgMemberDoesNotExist.hide();
            msgEnterValue.hide();
            msgSelectRecord.hide();
            msgErrorSaving.hide();
            msgSavedSuccessfully.hide();
        }
        function clearAllFields() {
            //Clear all fields
            txbDays.val('');
            //txbAttempts.val('');
            examRecordList = [];
            examRecordCbList = [];
            ddlGiveExamSelect.html('');
            tblExamRecordList.html('');
            //examRecordCbList = null;
        }
    });





//  Extend Subscriptions
//  ====================
//  Search 'click':
//  	if field[codeName] is blank
//  		show error msg
//  	else
//  		If member does not exists
//  			show error msg
//  		else
//  			Pull data [including exam IDs]
//  			populate fields

//  		Show "Exam Settings"



//  Give Exam "click"
//  	if exam does not exist for member
//  		add exam to member
//  		refresh data



//  Extend Selected 'click'
//  	if no items are checked
//  		show error msg
//  	else
//  		validate days/attempts if not empty
//  		if both are empty
//  			show error msg
//  		else
//  			if days
//  				add days to expiration date of selected item(s)
//  			if attempts
//  				add value to allowed attempts

//  			if successfull
//  				Update fields
//  				Show success msg
//  			else
//  				show error msg