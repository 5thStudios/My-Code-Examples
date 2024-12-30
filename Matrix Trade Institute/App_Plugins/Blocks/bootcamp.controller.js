angular.module("umbraco").controller("bootcampcontroller", function ($scope, $filter, contentResource) {

    //Instantiate variable that will be referenced in html
    var vm = this;


    //Obtain variables
    var dateFrom = $scope.block.data.dateFrom;
    var dateTo = $scope.block.data.dateTo;
    var timeFrom = $scope.block.data.timeFrom;
    var timeTo = $scope.block.data.timeTo;


    //Create title string with date range
    if ($filter('date')(new Date(dateFrom), 'MMMM') === $filter('date')(new Date(dateTo), 'MMMM')) {
        vm.Title =
            $filter('date')(new Date(dateFrom), 'MMMM d') +
            $filter('date')(new Date(dateTo), '-d') +
            "  |  " + timeFrom + "-" + timeTo;
    }
    else {
        vm.Title =
            $filter('date')(new Date(dateFrom), 'MMMM d') +
            $filter('date')(new Date(dateTo), '-MMMM d') +
            "  |  " + timeFrom + "-" + timeTo;
    }

});



/*
https://docs.umbraco.com/umbraco-cms/extending/content-apps#creating-the-view-and-the-controller
https://joe.gl/ombek/blog/umbraco-angularjs-filter-cheat-sheet/
*/