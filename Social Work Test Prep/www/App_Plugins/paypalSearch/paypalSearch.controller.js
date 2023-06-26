angular
    .module('umbraco')
    .controller('paypalSearchController', function ($scope) {


        //  INSTANTIATE VARIABLES
        /* buttons */
        var btnRecordSearch = $('#btnRecordSearch');

        /* text boxes & fields */
        var txbRecordId = $('#txbRecordId');
        var ddlGiveExamSelect = $('#ddlGiveExamSelect');
        var tblExamRecordList = $('#examRecordList');

        /* msgs & panels */
        var tblPaypalSearch = $('.tblPaypalSearch > tbody');
        var msgRecordDoesNotExist = $('.msgRecordDoesNotExist');

        /* vars  */
        var jsonData;
        var body = $('body');
        var urlPath = window.location.protocol + '//' + window.location.host;
        var internalApi = "";
        const months = ["Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sept", "Oct", "Nov", "Dec"];


        //Hide msgs and panels
        hideMsgs();




        //Handles
        $(btnRecordSearch).click(function (e) {
            //Hide msgs
            hideMsgs();


            //Proceed only if value has been entered
            if (txbRecordId.val()) {
                //Force cursor to show that the coupon search is in progress
                $(body).addClass('pending');


                //Create api call with data
                internalApi = urlPath + '/umbraco/Api/SwtpApi/PaypalSearchRecord?data=' + encodeURIComponent(txbRecordId.val());


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
                    hideMsgs();
                    tblPaypalSearch.hide();
                    alert("ERROR: " + result);

                    //Remove cursor override
                    $(body).removeClass('pending');
                }
                function ServiceSucceeded(searchResults) {
                    //Parse incoming data
                    jsonData = JSON.parse(searchResults.responseJSON);
                    console.log(jsonData);

                    //clear all fields
                    clearAllFields();
                    hideMsgs();


                    if (!jsonData.length) {
                        //Show msg
                        msgRecordDoesNotExist.show();
                    }
                    else {
                        //
                        tblPaypalSearch.show();

                        //Populate fields if data returned
                        $.each(jsonData, function () {
                            console.log(this);

                            //Convert expiration date to proper format [Mar 23, 2023]
                            var d = new Date(this.created);
                            var dd = d.getDate();
                            var mm = months[d.getMonth()];
                            var yy = d.getFullYear();
                            var createdDate = mm + ' ' + dd + ', ' + yy;


                            //Add tr to table
                            var opening = '<tr class="text-left examRecord">';

                            var lblCreatedDate = '<td><label>' + createdDate + '</label></td>';
                            var lblEmail = '<td><label>' + this.payerEmail + '</label></td>';
                            //var lblMemberId = '<td><label>' + this.memberId + '</label></td>';
                            var lblItemName = '<td><label>' + this.itemName + '</label></td>';
                            //var lblItemNumber = '<td><label>' + this.itemNumber + '</label></td>';
                            var lblPrice = '<td><label>' + this.price + '</label></td>';
                            var lblDiscount = '<td><label>' + this.discount + '</label></td>';

                            var closing = '</tr>';
                            tblExamRecordList.append(opening + lblCreatedDate + lblEmail + lblItemName + lblPrice + lblDiscount + closing);
                            //tblExamRecordList.append(opening + lblCreatedDate + lblEmail + lblMemberId + lblItemName + lblItemNumber + lblPrice + lblDiscount + closing);
                        });
                    }



                    //public DateTime ? created { get; set; }
                    //public string payerEmail { get; set; }
                    //public int ? memberId { get; set; }
                    //public string itemName { get; set; }
                    //public int ? itemNumber { get; set; }
                    //public decimal ? price { get; set; }
                    //public decimal ? discount { get; set; }

                    //<tr class="text-left examRecord">
                    //    <th>2016-07-07 22:06</th>
                    //    <th>payment@parachutewebdesign.com</th>
                    //    <th>180308</th>
                    //    <th>DSM Booster</th>
                    //    <th>4868130</th>
                    //    <th>0.01</th>
                    //    <th>19.99</th>
                    //</tr>



                    //Remove cursor override
                    $(body).removeClass('pending');
                }


                //Remove cursor override
                $(body).removeClass('pending');
            }

        });




        //Methods
        function hideMsgs() {
            //Hide all messages
            msgRecordDoesNotExist.hide();
            tblPaypalSearch.hide();
        }
        function clearAllFields() {
            tblExamRecordList.html('');
            //Clear all fields
            //examRecordList = [];
            //txbDays.val('');
            //txbAttempts.val('');
            //examRecordCbList = [];
            //ddlGiveExamSelect.html('');
            //examRecordCbList = null;
        }


    });
