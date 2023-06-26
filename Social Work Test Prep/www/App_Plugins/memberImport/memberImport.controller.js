angular
    .module('umbraco')
    .controller('memberImportController', function ($scope) {

        //Instantiate variables
        var btnImportToTable = $('#btnImportToTable');
        var btnImportExcel = $('#btnImportExcel');

        var lblMsgFailed = $('.memberImportController label.alertMsg-failed');
        var lblMsgSucceess = $('.memberImportController label.alertMsg-success');
        var divMsgResults = $('.memberImportController div.alertMsg-results');
        var dataPnl = $('.data-pnl');
        var importControls = $('.import-controls');
        var cbExtendOnly = $('#cbExtendOnly');
        var jsonData = "";


        //Methods
        function ExportToTable() {
            //
            var dataPnl = $('.data-pnl');
            var importPnl = $('.import-pnl');
            var lblAlertMsg = $('.alertMsg');
            var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.xlsx|.xls)$/;

            /*Checks whether the file is a valid excel file*/
            if (regex.test($("#excelfile").val().toLowerCase())) {
                /*Flag for checking whether excel is .xls format or .xlsx format*/
                var xlsxflag = false;

                if ($("#excelfile").val().toLowerCase().indexOf(".xlsx") > 0) {
                    xlsxflag = true;
                }

                /*Checks whether the browser supports HTML5*/
                if (typeof (FileReader) != "undefined") {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        var data = e.target.result;
                        var workbook;

                        /*Converts the excel data into an object*/
                        if (xlsxflag) {
                            workbook = XLSX.read(data, { type: 'binary' });
                        }
                        else {
                            workbook = XLS.read(data, { type: 'binary' });
                        }

                        /*Gets all the sheetnames of excel in to a variable*/
                        var sheet_name_list = workbook.SheetNames;

                        /*This is used for restricting the script to consider only first sheet of excel*/
                        var cnt = 0;
                        sheet_name_list.forEach(function (y) { /*Iterate through all sheets*/
                            /*Convert the cell value to Json*/
                            if (xlsxflag) {
                                var exceljson = XLSX.utils.sheet_to_json(workbook.Sheets[y]);
                            }
                            else {
                                var exceljson = XLS.utils.sheet_to_row_object_array(workbook.Sheets[y]);
                            }


                            if (exceljson.length > 0 && cnt == 0) {
                                BindTable(exceljson, '#exceltable');
                                dataPnl.show();
                                importPnl.hide();
                                cnt++;

                                //Store json data in hidden field
                                $('#hfldJson').val(JSON.stringify(exceljson));
                                jsonData = JSON.stringify(exceljson);
                                //console.log(exceljson);
                                //console.log($('#hfldJson').val());
                                //console.log(jsonData);
                            }
                        });
                        $('#exceltable').show();
                    }

                    /*If excel file is .xlsx extension than creates a Array Buffer from excel*/
                    if (xlsxflag) {
                        reader.readAsArrayBuffer($("#excelfile")[0].files[0]);
                    }
                    else {
                        reader.readAsBinaryString($("#excelfile")[0].files[0]);
                    }
                }
                else {
                    //alert("Sorry! Your browser does not support HTML5!");
                    $(lblAlertMsg).text("Sorry! Your browser does not support HTML5!");
                    lblAlertMsg.show();
                }
            }
            else {
                //alert("Please upload a valid Excel file!");
                $(lblAlertMsg).text("Please upload a valid Excel file!");
                lblAlertMsg.show();
            }
        }
        function BindTable(jsondata, tableid) {/*Function used to convert the JSON array to Html Table*/
            var columns = BindTableHeader(jsondata, tableid); /*Gets all the column headings of Excel*/
            for (var i = 0; i < jsondata.length; i++) {
                var row$ = $('<tr/>');
                //console.log('columns.length: ' + columns.length);
                for (var colIndex = 0; colIndex < columns.length; colIndex++) {
                    var cellValue = jsondata[i][columns[colIndex]];
                    if (cellValue == null) cellValue = "";
                    row$.append($('<td/>').html(cellValue));
                }
                $(tableid).append(row$);
            }
            BindTableFooter(jsondata, tableid);
            //$(tableid).append(BindTableFooter);
        }
        function BindTableHeader(jsondata, tableid) {/*Function used to get all column names from JSON and bind the html table header*/
            var columnSet = [];
            var headerTr$ = $('<thead><tr/>');
            for (var i = 0; i < jsondata.length; i++) {
                //json index map.  Only take the following columns
                var rowHash = jsondata[i];
                var j = 0;
                for (var key in rowHash) {

                    /*if (j == 0 || j == 2 || j == 4 || j == 6 || j == 7) {*/
                    if (rowHash.hasOwnProperty(key)) {
                        if ($.inArray(key, columnSet) == -1) {/*Adding each unique column names to a variable array*/
                            columnSet.push(key);
                            headerTr$.append($('<th/></thead>').html(key));
                        }
                    }
                    /*}*/
                    j++;
                }
            }
            $(tableid).append(headerTr$);
            return columnSet;
        }
        function BindTableFooter(jsondata, tableid) {/*Function used to count all records and bind to the html table footer*/
            var headerTr$ = $('<tfoot><tr><th colspan="5">Total: ' + jsondata.length + ' Records<th/></tr></tfoot>');
            $(tableid).append(headerTr$);
        }


        //Events
        $(btnImportToTable).click(function () {
            ExportToTable();
        });
        $(btnImportExcel).click(function () {
            /* Show cursor as spinning icon after clicking to import excel */
            $('body').css('cursor', 'wait');
            $('body *').css('cursor', 'wait');
            $('html').css('cursor', 'wait');
            $('div').css('cursor', 'wait');
            //$(btnImportExcel).css('display', 'none');
            $(importControls).hide();

            //Get value if we are to extend days only
            var extendOnly = $(cbExtendOnly).prop('checked');


            //Create data to pass to api
            //var data = CreateParameters();
            //function CreateParameters() {
            //    //Obtain all current checkboxes
            //    examRecordCbList = null;
            //    examRecordCbList = $('#examRecordList input.cbExamRecord:checked');


            //    //Update json data with updated info
            //    jsonData.BonusExamId = ddlGiveExamSelect.val();


            //    //Convert json to string for api submission
            //    return JSON.stringify(jsonData);
            //}


            //Create api call 
            var urlPath = window.location.protocol + '//' + window.location.host;
            var internalApi = urlPath + '/umbraco/Api/SwtpApi/ImportMembers';
            if (extendOnly) { internalApi = urlPath + '/umbraco/Api/SwtpApi/ExtendMembers'; }


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
                        data: jsonData, //data
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
                lblMsgFailed.show();
                               

                //Remove cursor override
                $('body').css('cursor', 'auto');
                $('body *').css('cursor', 'auto');
                $('html').css('cursor', 'auto');
                $('div').css('cursor', 'auto');
            }
            function ServiceSucceeded(searchResults) {
                console.log('Service call Succeeded');
                //jsonData = JSON.parse(searchResults.responseJSON);
                //console.log(jsonData);
                console.log(searchResults.responseJSON)

                //Show msg
                lblMsgSucceess.show();
                //divMsgResults.html(searchResults.responseJSON);
                divMsgResults.show();


                //Remove cursor override
                $('body').css('cursor', 'auto');
                $('body *').css('cursor', 'auto');
                $('html').css('cursor', 'auto');
                $('div').css('cursor', 'auto');
            }












        });
    });