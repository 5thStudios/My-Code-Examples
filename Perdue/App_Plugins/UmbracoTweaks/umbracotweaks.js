//ADD HOST NAME OF URL TO BODY TAG AS A CLASS
//========================================================
//jQuery(function ($) {
$(document).ready(function () {
    //$(window).on("load", function (e) {

    //Obtain host name
    var host = window.location.host.toLowerCase();


    //Add host name as class to body tag
    if (host.indexOf("localhost") >= 0) {
        $('body').addClass('localhost');

        // delay to allow pg render
        setTimeout(function () {
            $("#applications ul.sections").append('<li class="expand envLabel">LOCAL</li>');
        }, 1000);
    }
    else if (host.indexOf("dev-") >= 0) {
        $('body').addClass('dev');

        // delay to allow pg render
        setTimeout(function () {
            $("#applications ul.sections").append('<li class="expand envLabel">DEVELOPMENT</li>');
        }, 1000);
    }
    else if (host.indexOf("stage") >= 0 || host.indexOf("staging") >= 0 || host.indexOf("test") >= 0) {
        $('body').addClass('staging');

        // delay to allow pg render
        setTimeout(function () {
            $("#applications ul.sections").append('<li class="expand envLabel">STAGING</li>');
        }, 1000);
    }
    else {
        $('body').addClass('live');
        // delay to allow pg render
        setTimeout(function () {
            $("#applications ul.sections").append('<li class="expand envLabel">LIVE - PRODUCTION</li>');
        }, 1000);
    }

});

