/*   BIO-CARD   */
angular.module("umbraco").controller("bioCardController", function ($scope, contentResource, mediaResource) {

    //Instantiate variable that will be referenced in html
    var vm = this;


    //Get node and grab node data.
    var uid = $scope.block.data.bio;	//Get uid value from picker


    if (uid !== undefined && uid != 0) {
        contentResource.getById(uid).then(function (node) {
            //Obtain node url
            //vm.Url = node.urls[0].text;


            //Node has tabs.  Loop through each one.
            $(node.variants[0].tabs).each(function (i, tab) {

                //Loop through each property within tab
                $(tab.properties).each(function (j, property) {

                    if (property.alias == "image") {
                        mediaResource.getById(property.value[0].mediaKey)
                            .then(function (media) {
                                //console.log(media.mediaLink);
                                vm.ImgUrl = media.mediaLink;
                            });
                    }
                    else if (property.alias == "fullName") {
                        //console.log(property.value);
                        vm.Name = property.value;
                    }
                    else if (property.alias == "title") {
                        //console.log(property.value);
                        vm.Title = property.value;
                    }
                });
            });
        });
    }
});



/*   LOCATION-CARD   */
angular.module("umbraco").controller("locationCardController", function ($scope, contentResource, mediaResource) {

    //Instantiate variable that will be referenced in html
    var vm = this;


    //Get node and grab node data.
    var uid = $scope.block.data.location;	//Get uid value from picker


    if (uid !== undefined && uid != 0) {
        contentResource.getById(uid).then(function (node) {
            //Obtain node location
            vm.Location = node.variants[0].name;

            //Address parts
            var address1 = "";
            var address2 = "";
            var city = "";
            var state = "";
            var zip = "";


            //Node has tabs.  Loop through each one.
            $(node.variants[0].tabs).each(function (i, tab) {

                //Loop through each property within tab
                $(tab.properties).each(function (j, property) {

                    if (property.alias == "image") {
                        mediaResource.getById(property.value[0].mediaKey)
                            .then(function (media) {
                                //console.log(media.mediaLink);
                                vm.ImgUrl = media.mediaLink;
                            });
                    }
                    else if (property.alias == "address1") {
                        address1 = property.value;
                    }
                    else if (property.alias == "address2") {
                        address2 = property.value;
                    }
                    else if (property.alias == "city") {
                        city = property.value;
                    }
                    else if (property.alias == "state") {
                        state = property.value;
                    }
                    else if (property.alias == "zipCode") {
                        zip = property.value;
                    }
                });
            });


            //Add proper html tags to break address into proper lines
            if (address1.length > 0) {
                address1 = "<div>" + address1 + "</div>";
            }
            if (address2.length > 0) {
                address2 = "<div>" + address2 + "</div>";
            }
            if (city.length > 0) {
                city = city + ", ";
            }
            if (state.length > 0) {
                state = state + " ";
            }


            //Concatinate address
            vm.Address = address1 + address2 + "<div>" + city + state + zip + "</div>";


        });
    }
});



/*   NEWS-CARD   */
angular.module("umbraco").controller("newsCardController", function ($scope, contentResource, mediaResource) {

    //Instantiate variable that will be referenced in html
    var vm = this;


    //Get node and grab node data.
    var uid = $scope.block.data.news;	//Get uid value from picker


    if (uid !== undefined && uid != 0) {
        contentResource.getById(uid).then(function (node) {
            //Obtain node name
            vm.Title = node.variants[0].name;
            //console.log(vm.Name);


            //Node has tabs.  Loop through each one.
            $(node.variants[0].tabs).each(function (i, tab) {

                //Loop through each property within tab
                $(tab.properties).each(function (j, property) {

                    if (property.alias == "image") {
                        mediaResource.getById(property.value[0].mediaKey)
                            .then(function (media) {
                                //console.log(media.mediaLink);
                                vm.ImgUrl = media.mediaLink;
                            });
                    }
                    else if (property.alias == "datePosted") {
                        //console.log(property.value);
                        var datePosted = new Date(property.value);
                        vm.DatePosted = datePosted.toLocaleDateString('en-us', { year: "numeric", month: "short", day: "numeric" });
                    }
                    else if (property.alias == "description") {
                        //console.log(property.value);
                        vm.Description = property.value;
                    }
                });
            });
        });
    }
});



/*   PROJECT-CARD   */
angular.module("umbraco").controller("projectCardController", function ($scope, contentResource, mediaResource) {

    //Instantiate variable that will be referenced in html
    var vm = this;


    //Get node and grab node data.
    var uid = $scope.block.data.project;	//Get uid value from picker


    if (uid !== undefined && uid != 0) {
        contentResource.getById(uid).then(function (node) {
            //Obtain node name
            vm.Project = node.variants[0].name;
            //console.log(vm.Name);
            

            //Node has tabs.  Loop through each one.
            $(node.variants[0].tabs).each(function (i, tab) {

                //Loop through each property within tab
                $(tab.properties).each(function (j, property) {

                    if (property.alias == "images") {
                        mediaResource.getById(property.value[0].mediaKey)
                            .then(function (media) {
                                //console.log(media.mediaLink);
                                vm.ImgUrl = media.mediaLink;
                            });
                    }
                    else if (property.alias == "location") {
                        //console.log(property.value);
                        vm.Location = property.value;
                    }
                });
            });
        });
    }
});

