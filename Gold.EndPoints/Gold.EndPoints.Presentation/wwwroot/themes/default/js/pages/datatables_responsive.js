/* ------------------------------------------------------------------------------
*
*  # Responsive extension for Datatables
*
*  Specific JS code additions for datatable_responsive.html page
*
*  Version: 1.0
*  Latest update: Aug 1, 2015
*
* ---------------------------------------------------------------------------- */

$(function() {


    // Table setup
    // ------------------------------

    // Setting datatable defaults
    $.extend( $.fn.dataTable.defaults, {
        autoWidth: false,
        responsive: true,
        columnDefs: [{ 
            orderable: false,
            width: '100px',
            targets: [ 5 ]
        }],
        dom: '<"datatable-header"fl><"datatable-scroll-wrap"t><"datatable-footer"ip>',
        language: {
            search: '<span>جستجو:</span> _INPUT_',
            searchPlaceholder: 'متن خود را بنویسید...',
            lengthMenu: '<span>تعداد سطرها:</span> _MENU_',
            paginate: { 'first': 'اولین', 'last': 'آخرین', 'next': '&larr;', 'previous': '&rarr;' },
            emptyTable: 'داده ای در جدول وجود ندارد',
            info: "نمایش _START_ تا _END_ از _TOTAL_ سطر"
        }
        //drawCallback: function () {
        //    $(this).find('tbody tr').slice(-3).find('.dropdown, .btn-group').addClass('dropup');
        //},
        //preDrawCallback: function() {
        //    $(this).find('tbody tr').slice(-3).find('.dropdown, .btn-group').removeClass('dropup');
        //}
    });


    // Basic responsive configuration
    $('.datatable-responsive').DataTable({
        "paging": false,
        "filter": false,
        "info": false
    });


    //// Column controlled child rows
    //$('.datatable-responsive-column-controlled').DataTable({
    //    responsive: {
    //        details: {
    //            type: 'column'
    //        }
    //    },
    //    columnDefs: [
    //        {
    //            className: 'control',
    //            orderable: false,
    //            targets:   0
    //        },
    //        { 
    //            width: "100px",
    //            targets: [6]
    //        },
    //        { 
    //            orderable: false,
    //            targets: [6]
    //        }
    //    ],
    //    order: [1, 'asc']
    //});


    //// Control position
    //$('.datatable-responsive-control-right').DataTable({
    //    responsive: {
    //        details: {
    //            type: 'column',
    //            target: -1
    //        }
    //    },
    //    columnDefs: [
    //        {
    //            className: 'control',
    //            orderable: false,
    //            targets: -1
    //        },
    //        { 
    //            width: "100px",
    //            targets: [5]
    //        },
    //        { 
    //            orderable: false,
    //            targets: [5]
    //        }
    //    ]
    //});


    //// Whole row as a control
    //$('.datatable-responsive-row-control').DataTable({
    //    responsive: {
    //        details: {
    //            type: 'column',
    //            target: 'tr'
    //        }
    //    },
    //    columnDefs: [
    //        {
    //            className: 'control',
    //            orderable: false,
    //            targets:   0
    //        },
    //        { 
    //            width: "100px",
    //            targets: [6]
    //        },
    //        { 
    //            orderable: false,
    //            targets: [6]
    //        }
    //    ],
    //    order: [1, 'asc']
    //});

    
    
});
