angular.module("umbraco").controller("CustomWelcomeDashboardController", function ($scope, userService, contentResource, logResource, entityResource) {
    var vm = this;

    //console.log('controller...');
    //console.log(vm);
    //console.log($scope);
    //console.log(userService);
    //console.log(contentResource);
    //console.log(logResource);
    //console.log(entityResource);
    //console.log('================================');

    vm.UserName = "guest";
    vm.UserLogHistory = []; 
    var userLogOptions = {
        pageSize: 50,
        pageNumber: 1,
        orderDirection: "Descending",
        sinceDate: new Date(2023, 0, 1)
    };

    var user = userService.getCurrentUser().then(function (user) {
        //console.log(user);
        vm.UserName = user.name;
    });



    logResource.getPagedUserLog(userLogOptions)
        .then(function (response) {
            //console.log(response);
            vm.UserLogHistory = response;

            // define the entity types that we care about, in this case only content and media
            var supportedEntityTypes = ["Document"]; //, "Media"

            // define an empty array "nodes we know about"
            var nodesWeKnowAbout = [];

            // define an empty array "filtered log entries"
            var filteredLogEntries = [];

            // loop through the entries in the User Log History
            angular.forEach(response.items, function (item) {

                // if the item is already in our "nodes we know about" array, skip to the next log entry
                if (nodesWeKnowAbout.includes(item.nodeId)) {
                    return;
                }

                // if the log entry is not for an entity type that we care about, skip to the next log entry
                if (!supportedEntityTypes.includes(item.entityType)) {
                    return;
                }

                // if the user did not save or publish, skip to the next log entry
                if (item.logType !== "Save" && item.logType !== "Publish") {
                    return;
                }

                // if the item does not have a valid nodeId, skip to the next log entry
                if (item.nodeId < 0) {
                    return;
                }

                // now, push the item's nodeId to our "nodes we know about" array
                nodesWeKnowAbout.push(item.nodeId);

                // use entityResource to retrieve details of the content/media item
                var ent = entityResource.getById(item.nodeId, item.entityType).then(function (ent) {
                    console.log(ent);
                    item.Content = ent;
                });

                // get the edit url
                if (item.entityType === "Document") {
                    item.editUrl = "content/content/edit/" + item.nodeId;
                }
                if (item.entityType === "Media") {
                    item.editUrl = "media/media/edit/" + item.nodeId;
                }

                // push the item to our "filtered log entries" array
                filteredLogEntries.push(item);

                // end of loop
            });

            // populate the view with our "filtered log entries" array
            vm.UserLogHistory.items = filteredLogEntries;

            // end of function
        });
});