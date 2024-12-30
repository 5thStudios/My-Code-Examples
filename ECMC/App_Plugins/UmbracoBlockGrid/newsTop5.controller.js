angular.module("umbraco").controller("newsTop5controller", function ($scope, contentResource) {
	//Instantiate variable to reference in html
	var vm = this;

	//Get uid value from newsPicker
	var newsPickerId = $scope.block.data.newsPicker;


	//Get node and grab node name.  [Created new parameter "NodeName" to store data in.]
	if (newsPickerId !== undefined) {
		contentResource.getById(newsPickerId).then(function (node) {
			//Get node name
			vm.NodeName = node.variants[0].name;

			//Get # to show
			var count = $scope.block.data.howManyToShow;
			if (count === undefined || count === '' || count === 0) {
				count = 5;
			}
			vm.howManyToShow = count;
		});
	}
});



/*
https://docs.umbraco.com/umbraco-cms/extending/content-apps#creating-the-view-and-the-controller
https://joe.gl/ombek/blog/umbraco-angularjs-filter-cheat-sheet/
*/