'use strict';
//  Author: AdminDesigns.com
// 
//  This file is reserved for changes made by the use. 
//  Always seperate your work from the theme. It makes
//  modifications, and future theme updates much easier 
// 

//(function ($) {
//    var loc = window.location;
//    var pathName = loc.href.substring(loc.href.length, loc.href.lastIndexOf('/') + 1);
//    $("a").each(function () {
//        var thisHref = $(this).attr('href');
//        if (thisHref == "/" + pathName) {
//            $(this).parent().parent().parent().children(".accordion-toggle").eq(0).addClass('menu-open');
//            $(this).parent().addClass('active');
//        }
//        else if (loc.href.includes("SearchStudentByNationalCode")) {
//            if (thisHref.includes("SearchStudentByNationalCode")) {
//                $(this).parent().parent().parent().children(".accordion-toggle").eq(0).addClass('menu-open');
//                $(this).parent().addClass('active');
//            }
//        }
//        else if (thisHref != "/" && loc.href.includes(thisHref)) {
//            $(this).parent().parent().parent().children(".accordion-toggle").eq(0).addClass('menu-open');
//            $(this).parent().addClass('active');
//        }
//    });

//})(jQuery);

//function ConfirmDelete(controller, id) {
//    var x = confirm("آیا از حذف آیتم انتخابی خود اطمینان دارید؟");
//    if (x) {
//        $.ajax({
//            url: '/' + controller + '/Delete',
//            data: { id: id },
//            success: function () {
//                $('#i' + id).fadeOut(600, function () {
//                    $(this).remove();
//                });
//            },
//            type: 'POST'
//        });
//    }
//}


//-----------------------------------------------------------------DeleteProduct-----------------------------------------------------
function ConfirmDelete(controller, id) {
    ShowConfirmAlert('آیا مطمئن به حذف هستید؟', 'شما قادر نخواهد بود برای بازیابی این آیتم کاری انجام دهید...', function (isConfirm) {
        if (isConfirm) {
            SendPostRequestWithAntiforgery('/' + controller + '/Delete', { id: id }, function (data) {
                $('#i' + id).fadeOut(600, function () {
                    $(this).remove();
                });
                refreshGrid("#grid_" + controller);
                OpenWindowCreate('/' + controller + '/Create', null);
            })
        }
    });
}

function ConfirmDeleteFullUrl(controller , action, id) {
    ShowConfirmAlert('آیا مطمئن به حذف هستید؟', 'شما قادر نخواهد بود برای بازیابی این آیتم کاری انجام دهید...', function (isConfirm) {
        if (isConfirm) {
            SendPostRequestWithAntiforgery('/' + controller + '/' + action , { id: id }, function (data) {
                $('#i' + id).fadeOut(600, function () {
                    $(this).remove();
                });
            })
        }
    });
}
