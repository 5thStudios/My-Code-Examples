angular
    .module('umbraco')
    .controller('apiImportsController', function ($scope) {




        //Instantiate variables
        var btnPullData = $('.apiImportsController .btnPullData');
        var lblMsg = $('.apiImportsController label.msgPullExams');
        var hfldJson = $('#hfldJson');
        var externalApi = "";
        var internalApi = "";
        var form;


        //Handles
        $("#submitData").submit(function (e) {
            e.preventDefault();

            //Is FormData initialized?
            if (window.FormData) { console.log('FormData initialized'); }
            else { console.log('FormData not supported!'); }

            //Get clicked button's api values
            var btn = e.originalEvent.submitter;
            externalApi = $(btn).data("external-api");
            internalApi = $(btn).data("internal-api");
            console.log('externalApi: ' + externalApi);
            console.log('internalApi: ' + internalApi);

            //Pass form data to api call
            form = this;

            //Call webservices
            if (externalApi.length > 0) {
                //Import data and then push to local site
                $(lblMsg).html('Pulling data from api...');
                PullData();
            }
            else {
                //Bypass import and run local function
                RunLocalFunction();
            }
        });


        // Read a page's GET URL variables and return them as an associative array.
        function getUrlVars() {
            var vars = [], hash;
            //var querystrings = 
            var hashes = externalApi.slice(externalApi.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }


        function PullData() {
            //Instantiate variables
            var data = CreateParameters();


            //Call AJAX service
            var response = CallService_POST();
            var promise = $.when(response);
            promise.done(function () { ServiceSucceeded(response); });
            promise.fail(function () { ServiceFailed(response); });



            //METHODS
            function CreateParameters() {
                //Instantiate an array of parameters to pass to handler
                var myData = {};

                //Split querystrings if exists.  [may only be finding 1st one for now.  update if needed.]
                if (externalApi.indexOf('?') != -1) {
                    var queries = getUrlVars();
                    var promise = $.when(queries);
                    promise.done(function () {
                        //console.log(queries[0]);
                        //console.log(queries[queries[0]]);                   
                        myData[queries[0]] = queries[queries[0]];
                    });
                }

                return JSON.stringify(myData);
                //return myData; 
            }
            function CallService_POST() {
                try {
                    return $.ajax({
                        url: externalApi,
                        type: "POST",
                        data: data,
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        processData: true
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

                //Error message
                console.log('Error Msg:');
                console.log(result);

                //Show message
                $(lblMsg).html("ERROR: " + result);
            }
            function ServiceSucceeded(searchResults) {
                //console.log('Data pulled successfully!!!');
                //console.log(JSON.parse(searchResults.responseText).d);
                //console.log('===================================================');

                //Show message
                $(lblMsg).html("Data pulled successfully.  Preparing to import data...");

                //Save json into hidden field.
                $(hfldJson).val(JSON.parse(searchResults.responseText).d);

                //Pass form data to api call
                formData = new FormData(form);

                //Import data into new site
                ImportData();
                function ImportData() {
                    //
                    //var data = CreateParameters();

                    //Call AJAX service
                    var response = CallService_POST();
                    var promise = $.when(response);
                    promise.done(function () { ServiceSucceeded(response); });
                    promise.fail(function () { ServiceFailed(response); });


                    //METHODS
                    function CallService_POST() {
                        try {
                            //return $.ajax({
                            //    url: internalApi,
                            //    type: "GET",
                            //    data: formData,
                            //    dataType: "json",
                            //    contentType: "application/json; charset=utf-8",
                            //    processData: true
                            //});

                            return $.ajax({
                                url: internalApi,
                                type: "POST",
                                data: formData,
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
                        $(lblMsg).html("ERROR: " + result);
                    }
                    function ServiceSucceeded(searchResults) {
                        console.log('Data submitted successfully!!!');
                        console.log(JSON.parse(searchResults.responseText));
                        console.log('===================================================');

                        //Show message
                        $(lblMsg).html('Data submitted successfully!!!');
                    }
                }
            }
        }

        function RunLocalFunction() {
            //Show message
            $(lblMsg).html('Running function...');

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
                $(lblMsg).html("ERROR: " + result);
            }
            function ServiceSucceeded(searchResults) {
                console.log('Data updated successfully!!!');
                console.log(JSON.parse(searchResults.responseText));
                console.log('===================================================');

                //Show message
                $(lblMsg).html('Data updated successfully!!!');
            }
        }



    });