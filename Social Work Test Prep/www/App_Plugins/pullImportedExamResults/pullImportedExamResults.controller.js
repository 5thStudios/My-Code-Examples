angular
    .module('umbraco')
    .controller('pullImportedExamResultsController', function ($scope) {

        //  INSTANTIATE VARIABLES
        /* buttons */
        var btnMemberSearch = $('#btnMemberSearch');

        /* text boxes & fields */
        var txbMemberEmail = $('#txbMemberEmail');
        var tblResults = $('#tblResults');
        var tblExamRecordList = $('#examRecordList');

        /* msgs & panels */
        var msgMemberDoesNotExist = $('.msgMemberDoesNotExist');
        var msgComplete = $('.msgComplete');
        var msgPullingData = $('.msgPullingData');

        /* vars  */
        var jsonData;
        var bodyTag = $('body');
        var urlPath = window.location.protocol + '//' + window.location.host;
        var internalApi = "";
        var examRecordList = [];


        //Hide msgs and panels
        hideMsgs();
        tblResults.hide();




        //Handles
        $(btnMemberSearch).click(function (e) {
            //Hide msgs
            hideMsgs();

            //Proceed only if value has been entered
            if (txbMemberEmail.val()) {
                //Force cursor to show that the coupon search is in progress
                $(bodyTag).addClass('pending');
                msgPullingData.show();
                console.log('Searching for member...');


                //Create api call with data
                internalApi = urlPath + '/umbraco/Api/SwtpApi/PullExamResultsIntoMember?data=' + encodeURIComponent(txbMemberEmail.val());


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
                    msgMemberDoesNotExist.show();

                    //Remove cursor override
                    $(bodyTag).removeClass('pending');
                }
                function ServiceSucceeded(searchResults) {
                    //Parse incoming data
                    jsonData = JSON.parse(searchResults.responseJSON);
                    console.log(jsonData);

                    //clear all fields and show complete msg.
                    clearAllFields();
                    hideMsgs();
                    msgComplete.show();
                    tblResults.show();

                    //Populate table if data returned
                    $.each(jsonData, function () {
                        //Add tr to table
                        var opening = '<tr class="text-left examRecord">';
                        var lblKey = '<td><label>' + this.StrKey + '</label></td>';
                        var lblValue = '<td><label>' + this.StrValue + '</label></td>';
                        var closing = '</tr>';
                        tblExamRecordList.append(opening + lblKey + lblValue + closing);
                    });


                    //Remove cursor override
                    $(bodyTag).removeClass('pending');
                }


                //Remove cursor override
                $(bodyTag).removeClass('pending');
            }

        });



        function hideMsgs() {
            //Hide all messages
            msgMemberDoesNotExist.hide();
            msgComplete.hide();
            tblResults.hide();
            msgPullingData.hide();
        }
        function clearAllFields() {
            //Clear all fields
            examRecordList = [];
            tblExamRecordList.html('');
        }
    });