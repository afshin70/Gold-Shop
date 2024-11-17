var handler = function () {
    if ($(this).val() != "" && $("#EnterLang").val() == "0") {
        $.post("/ConvertDates/ConvertDate", { id: $(this).val() }, function (data) {
            document.getElementById('FromDate').value = data;
        });
    }
};
var handlerExit = function () {
    if ($(this).val() != "" && $("#ExitLang").val() == "0") {
        $.post("/ConvertDates/ConvertDate", { id: $(this).val() }, function (data) {
            document.getElementById('UpToDate').value = data;
        });
    }
};
$('#FromDateStr').on('change', handler);
$('#UpToDateStr').on('change', handlerExit);
var
    persianNumbers = [/۰/g, /۱/g, /۲/g, /۳/g, /۴/g, /۵/g, /۶/g, /۷/g, /۸/g, /۹/g],
    arabicNumbers = [/٠/g, /١/g, /٢/g, /٣/g, /٤/g, /٥/g, /٦/g, /٧/g, /٨/g, /٩/g],
    fixNumbers = function (str) {
        if (typeof str === 'string') {
            for (var i = 0; i < 10; i++) {
                str = str.replace(persianNumbers[i], i).replace(arabicNumbers[i], i);
            }
        }
        return str;
    };
$('#FromDate').change(function () {
    if ($(this).val() != "") {
        $("#EnterLang").val("1");
        var selectedDate = new Date($(this).val()).toLocaleDateString('fa-IR');
        var date = (selectedDate).split('/');
        y = Number(fixNumbers(date[0]));
        m = Number(fixNumbers(date[1]));
        d = Number(fixNumbers(date[2]));
        if (m < 10) {
            m = '0' + m;
        } if (d < 10) {
            d = '0' + d;
        }
        $('#FromDateStr').persianDatepicker({
            timePicker: { enabled: false }, format: 'YYYY/MM/DD'
        }).pDatepicker('setDate', [y, parseInt(m), parseInt(d)]);
    }
});

$('#UpToDate').change(function () {
    if ($(this).val() != "") {
        $("#ExitLang").val("1");
        var selectedDate = new Date($(this).val()).toLocaleDateString('fa-IR');
        var date = (selectedDate).split('/');
        y = Number(fixNumbers(date[0]));
        m = Number(fixNumbers(date[1]));
        d = Number(fixNumbers(date[2]));
        if (m < 10) {
            m = '0' + m;
        } if (d < 10) {
            d = '0' + d;
        }
        $('#UpToDateStr').persianDatepicker({
            timePicker: { enabled: false }, format: 'YYYY/MM/DD'
        }).pDatepicker('setDate', [y, parseInt(m), parseInt(d)]);
    }
});

function OpenWindowCreate(createUrl, createParameter) {
    if (createParameter != null) {
        $.get(createUrl,
            createParameter,
            function (data) {
                $("#forms-body").html(data);
                $('.date').val('');
                $('.time').val('');
                $('#FromDateStr').persianDatepicker({
                    timePicker: { enabled: false }, format: 'YYYY/MM/DD'
                }).pDatepicker('setDate', [y, parseInt(m), parseInt(d)]);
                $('#UpToDateStr').persianDatepicker({
                    timePicker: { enabled: false }, format: 'YYYY/MM/DD'
                }).pDatepicker('setDate', [yExit, parseInt(mExit), parseInt(dExit)]);
            });
    } else {
        $.get(createUrl, null, function (data) {
            $("#forms-body").html(data);
            $('.date').val('');
            $('.time').val('');
            $('#FromDateStr').persianDatepicker({
                timePicker: { enabled: false }, format: 'YYYY/MM/DD'
            }).val('');
            $('#UpToDateStr').persianDatepicker({
                timePicker: { enabled: false }, format: 'YYYY/MM/DD'
            }).val('');
        });
    }

}


$("#FromDate").click(function () {
    setTimeout(function () {
        if ($("#FromDate").val() == "") {
            $("#FromDateStr").val('')
        }
    }, 600);
})

$("#UpToDate").click(function () {
    setTimeout(function () {
        if ($("#UpToDate").val() == "") {
            $("#UpToDateStr").val('')
        }
    }, 600);
})




