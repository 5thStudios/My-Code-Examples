//Initialize zurb foundation
$(document).foundation()


//Initialize slick slider
$(document).ready(function () {
    $('.testimonials').slick({
        autoplay: true,
        autoplaySpeed: 2000,
        dots: true,
        infinite: true,
        arrows: false,
        customPaging: function (slider, i) {
            return '<a><img src="/images/common/circle.png" /><img src="/images/common/circle-full.png" /></a>';
        },
    });
});


//Isotope functionality
$(document).ready(function () {
    //Initialize variables    
    var projects = $('.projects');
    var lst = $('#gallery-filters input[type=radio]');


    //Initialize Isotope
    projects.isotope({
        itemSelector: '.project'
    });

    //Handle events
    lst.click(function () {
        var filterValue = $(this).attr('data-filter');
        projects.isotope({ filter: filterValue });
        console.log(filterValue);
    });
});




//Scroll to contact form after submitting form
$(document).ready(function () {
    if ($('#hfldScrollTo').length) {
        var aTag = $("a[name='contact']");
        $('html,body').animate({ scrollTop: aTag.offset().top }, 'slow');
    }
});



//Set up recpatcha and submit function
//=================================

//Instantiate variable
var notARobot = false;
var recaptchaContainer = $('#RecaptchaContainer');
var lblRecaptchaMessage = $('#lblRecaptchaMessage');
var recaptchaKey = $('#hfldRecaptchaKey').val();

var renderRecaptcha = function () {
    if ($(recaptchaContainer).length > 0) {
        grecaptcha.render('RecaptchaContainer', {
            'sitekey': recaptchaKey,
            'callback': reCaptchaCallback,
            theme: 'light', //light or dark
            size: 'normal'//normal or compact
        });
    }
};
var reCaptchaCallback = function (response) {
    if (response !== '') {
        notARobot = true;
        recaptchaContainer.removeClass('alert');
        lblRecaptchaMessage.hide();
    }
};

jQuery('input[type="submit"]').click(function (e) {
    if (notARobot) {
        //Valid user.  Submit form.
        return true;
    }
    else {
        //A Borg!!!  Resistance is futile!!
        e.preventDefault();
        recaptchaContainer.addClass('alert');
        lblRecaptchaMessage.show();
        return false;
    }
});