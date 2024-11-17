/* ------------------------------------------------------------------------------
*
*  # Stepy wizard
*
*  Specific JS code additions for wizard_stepy.html page
*
*  Version: 1.1
*  Latest update: Dec 25, 2015
*
* ---------------------------------------------------------------------------- */

$(function() {


    // Override defaults
    $.fn.stepy.defaults.legend = false;
    $.fn.stepy.defaults.transition = 'fade';
    $.fn.stepy.defaults.duration = 150;
    $.fn.stepy.defaults.backLabel = '<i class="icon-arrow-left13 position-left"></i> Back';
    $.fn.stepy.defaults.nextLabel = 'Next <i class="icon-arrow-left13 position-right"></i>';



    // Wizard examples
    // ------------------------------

    // Basic wizard setup
    $(".stepy-basic").stepy();


    // Hide step description
    $(".stepy-no-description").stepy({
        description: false
    });


    // Clickable titles
    $(".stepy-clickable").stepy({
        titleClick: true
    });


    // Stepy callbacks
    $(".stepy-callbacks").stepy({
        next: function(index) {
            alert('Going to step: ' + index);
        },
        back: function(index) {
            alert('Returning to step: ' + index);
        },
        finish: function() {
            alert('Submit canceled.');
            return false;
        }
    });


    //
    // Validation
    //

    // Initialize wizard
    $(".stepy-validation").stepy({
        titleClick: true,
        validate: true,
        block: true,
        next: function(index) {
            if (!$(".stepy-validation").validate(validate)) {
                return false;
            }
        },
        backLabel: ' مرحله قبل' ,
        nextLabel: 'مرحله بعد ' 
    });


    // Initialize validation
    var validate = {
        ignore: 'input[type=hidden], .select2-search__field', // ignore hidden fields
        errorClass: 'validation-error-label',
        successClass: 'validation-valid-label',
        highlight: function(element, errorClass) {
            $(element).removeClass(errorClass);
        },
        unhighlight: function(element, errorClass) {
            $(element).removeClass(errorClass);
        },

        // Different components require proper error label placement
        errorPlacement: function(error, element) {

            // Styled checkboxes, radios, bootstrap switch
            if (element.parents('div').hasClass("checker") || element.parents('div').hasClass("choice") || element.parent().hasClass('bootstrap-switch-container') ) {
                if(element.parents('label').hasClass('checkbox-inline') || element.parents('label').hasClass('radio-inline')) {
                    error.appendTo( element.parent().parent().parent().parent() );
                }
                 else {
                    error.appendTo( element.parent().parent().parent().parent().parent() );
                }
            }

            // Unstyled checkboxes, radios
            else if (element.parents('div').hasClass('checkbox') || element.parents('div').hasClass('radio')) {
                error.appendTo( element.parent().parent().parent() );
            }

            // Input with icons and Select2
            else if (element.parents('div').hasClass('has-feedback') || element.hasClass('select2-hidden-accessible')) {
                error.appendTo( element.parent() );
            }

            // Inline checkboxes, radios
            else if (element.parents('label').hasClass('checkbox-inline') || element.parents('label').hasClass('radio-inline')) {
                error.appendTo( element.parent().parent() );
            }

            // Input group, styled file input
            else if (element.parent().hasClass('uploader') || element.parents().hasClass('input-group')) {
                error.appendTo( element.parent().parent() );
            }

            else {
                error.insertAfter(element);
            }
        },
        rules: {
            email: {
                email: true
            },
            messages: {
                required: "وارد کردن فیلد الزامی است",
                email: {
                    required: "لطفا ایمیل خود را وارد کنید",
                    email: "فرمت ایمیل را درست وارد کنید مثل: name@domain.com"
                }
            }

        }
    }

    jQuery.extend(jQuery.validator.messages, {
        required: "وارد کردن این فیلد الزامی است.",
       
        email: "لطفا آدرس ایمیل معتبر وارد کنید.",
        url: "Please enter a valid URL.",
        date: "Please enter a valid date.",
        dateISO: "Please enter a valid date (ISO).",
        number: "لطفا عدد معتبر وارد کنید.",
        digits: "لطفا فقط اعداد را وارد کنید.",
        creditcard: "Please enter a valid credit card number.",
        equalTo: "Please enter the same value again.",
        accept: "Please enter a value with a valid extension.",
        maxlength: jQuery.validator.format("لطفا بیشتر از  {0} کاراکتر وارد نکنید."),
        minlength: jQuery.validator.format("Please enter at least {0} characters."),
        rangelength: jQuery.validator.format("لطفا تعداد کاراکتر بین {0} و {1} وارد کنید."),
        range: jQuery.validator.format("لطفا مقداری بین {0} و {1} وارد کنید."),
        max: jQuery.validator.format("لطفا مقداری کمتر یا مساوی {0}وارد کنید."),
        min: jQuery.validator.format("لطفا مقداری بیشتر یا مساوی {0}وارد کنید.")
    });

    // Initialize plugins
    // ------------------------------

    // Apply "Back" and "Next" button styling
    $('.stepy-step').find('.button-next').addClass('btn btn-primary');
    $('.stepy-step').find('.button-back').addClass('btn btn-default');


   

   
    
});
