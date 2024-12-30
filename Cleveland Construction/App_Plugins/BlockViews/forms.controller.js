/*   FORMS   */
angular.module("umbraco").controller("formController", function ($scope, formPickerResource) {
    //Instantiate variable that will be referenced in html
    var vm = this;
    vm.uid = "";
    vm.selectedForm = "";
    vm.selectedForm.icon = "";



    //Get node and grab node data.
    vm.uid = $scope.block.data.formPicker;	//Get uid value from picker


    //Only do this is we have a value saved
    if (vm.uid) {

        formPickerResource.getPickedForm(vm.uid).then(function (response) {
            vm.selectedForm = response;
        }, function (err) {
            //The 500 err will get caught by UmbRequestHelper & show the stacktrace in YSOD dialog if in debug or generic red error to see logs

            //Got an error from the HTTP API call
            //Most likely cause is the picked/saved form no longer exists & has been deleted
            //Need to bubble this up in the UI next to prop editor to make it super obvious

            //Using Angular Copy - otherwise the data binding will be updated
            //So the error message wont make sense, if the user then updates/picks a new form as the model.value will update too
            var currentValue = angular.copy(vm.uid);

            //Put something in the prop editor UI - some kind of red div or text
            vm.error = "The saved/picked form with id '" + currentValue + "' no longer exists. Pick another form below or clear out the old saved form";
        });
    }
});
