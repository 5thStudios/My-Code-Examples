angular.module("umbraco").controller("relatedNewscontroller", function ($scope, contentResource) {

    //Instantiate variable that will be referenced in html
    var vm = this;


    //Get node and grab node name.
    var rootListId = $scope.block.data.rootList;	//Get uid value from newsPicker
    if (rootListId !== undefined) {
        contentResource.getById(rootListId).then(function (node) {
            vm.RootListName = node.variants[0].name;
        });
    }


    //Get list and add to array.
    var tagsAreasOfInterestId = $scope.block.data.tagsAreasOfInterest;
    vm.Tags = [];
    if (tagsAreasOfInterestId !== undefined && tagsAreasOfInterestId !== '') {
        var tags = tagsAreasOfInterestId.split(',')

        tags.forEach(function (tag) {
            contentResource.getById(tag).then(function (node) {
                vm.Tags.push(node.variants[0].name);
                //console.log(node.variants[0].name);
            });
        });
    }
    else {
        vm.Tags.push("None Selected");
    }

});



/*
https://docs.umbraco.com/umbraco-cms/extending/content-apps#creating-the-view-and-the-controller
https://joe.gl/ombek/blog/umbraco-angularjs-filter-cheat-sheet/
*/