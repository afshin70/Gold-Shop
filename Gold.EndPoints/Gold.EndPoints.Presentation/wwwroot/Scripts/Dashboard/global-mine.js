/**
 * All Global Functions
 * @author Mohammad R.P - @[@
 */
// jQuery plugin example:

//error messages
let errorInProccessDataInServer = 'خطایی در سرور رخ داده است';
let errorInConnectToServer = 'خطا در ارتباط با سرور';

//end--error messages

$(document).on('click', '.password-icon', (e) => {
    let icon = $(e.currentTarget);
    let objInput = icon.parent().parent().find('input');
    if (icon.hasClass('active')) {
        icon.addClass('fa-eye-slash');
        icon.removeClass('active').removeClass('fa-eye');
        objInput.attr('type', 'password');

    } else {
        icon.removeClass('fa-eye-slash');
        icon.addClass('active fa-eye');
        objInput.attr('type', 'text');
    }
});

$(document).ready(function () {




    $('.onlydigit').keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

    $("nav.main .wrapper  ul  li").click(function () {
        var is = $(this).find('ul').css('visibility');
        $("nav.main .wrapper > ul > li ul").css('display', 'block');
        $("nav.main .wrapper > ul > li ul").css('opacity', '0');
        $("nav.main .wrapper > ul > li ul").css('visibility', 'hidden');

        console.log(is);
        if (is == 'hidden') {
            $(this).find('ul').css('display', 'block');
            $(this).find('ul').css('opacity', '1');
            $(this).find('ul').css('visibility', 'visible');
        } else {
            $(this).find('ul').css('display', 'block');
            $(this).find('ul').css('opacity', '0');
            $(this).find('ul').css('visibility', 'hidden');
        }

        //return false;
    })

    setSwitcheryStyle()

});

$(document).click(function (e) {
    if ($(e.target).is('nav, nav *')) {
        return;
    }

    $("nav.main .wrapper > ul > li ul").css('display', 'block');
    $("nav.main .wrapper > ul > li ul").css('opacity', '0');
    $("nav.main .wrapper > ul > li ul").css('visibility', 'hidden');

});

$('.priceComma').keypress(function (e) {
    if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
        return false;
    }
});

function commaSeparateNumber(val) {
    while (/(\d+)(\d{3})/.test(val.toString())) {
        val = val.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
    }
    val = val.toString();
    if (val.includes('.')) {

        var indexDesimal = val.indexOf('.');

        return val.substring(0, indexDesimal + 3);
    } else {
        return val;
    }
}

function slashSeparateNumber(val) {
    while (/(\d+)(\d{3})/.test(val.toString())) {
        val = val.toString().replace(/(\d+)(\d{3})/, '$1' + '/' + '$2');
    }
    val = val.toString();
    if (val.includes('.')) {

        var indexDesimal = val.indexOf('.');

        return val.substring(0, indexDesimal + 3);
    } else {
        return val;
    }
}

$(".commaseprator").keyup(function (e) {
    moneyCommaSep($(this));
});
function moneyCommaSep(ctrl) {

    var separator = ",";
    var int = ctrl.val().replace(new RegExp(separator, "g"), "");
    var regexp = new RegExp("\\B(\\d{3})(" + separator + "|$)");
    do {
        int = int.replace(regexp, separator + "$1");
    }
    while (int.search(regexp) >= 0)

    ctrl.val(int);

}

function commaCondition(val) {
    if (!Number.isInteger(val)) {
        return commaSeparateNumber(val);
    }
    else {
        var number = val.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        return number;
    }
}

function GetPriceStyle(Price) {
    if (Price == null) {
        return "";
    }

    else if (Price.toString().length < 4) {
        return Price;
    }

    return commaSeparateNumber(Price);
}
function GetPriceStyleWithSlash(Price) {
    if (Price == null) {
        return "";
    }

    else if (Price.toString().length < 4) {
        return Price;
    }

    return slashSeparateNumber(Price);
}


function CloseKendoWindow(name) {
    var window = $("#" + name).data("kendoWindow");
    window.close();
}

function OpenKendoWindow(name) {
    var window = $("#" + name).data("kendoWindow");
    window.open();
}

$(document).ready(function () {

    $(function () {
        $('form').each(function () {
            $(this).find('div.form-group').each(function () {
                if ($(this).find('span.field-validation-error').length > 0) {
                    $(this).addClass('has-error');
                }
            });
            $(this).find('div.input-group').each(function () {
                if ($(this).find('span.field-validation-error').length > 0) {
                    $(this).addClass('has-error');
                }
            });
        });
    });
    $(function () {

    });



});





function ShowValidationAlertToast(type, targetId, validations) {

    var msg = "<ul style='list-style-type: none;'>";
    $.each(validations, function (key, value) {
        var tt = "<li>" + value.Message + "</li>"
        msg += tt;
    })

    ShowAlertToast(type, '', msg);
}

function ShowAlertToast(type, ttl, message) {
    var shortCutFunction = '';
    var toastCount = 0;
    if (type == 0) {
        shortCutFunction += 'success';
    } else if (type == 1) {
        shortCutFunction += 'error';
    } else if (type == 2) {
        shortCutFunction += 'warning';
    } else if (type == 3) {
        shortCutFunction += 'info';
    }
    //var shortCutFunction = $("#toastTypeGroup input:checked").val();
    var msg = message;
    var title = ttl;
    var $showDuration = '1000';
    var $hideDuration = '1000';
    var $timeOut = '5000';
    var $extendedTimeOut = '1000';
    var $showEasing = 'swing';
    var $hideEasing = 'linear';
    var $showMethod = 'fadeIn';
    var $hideMethod = 'fadeOut';
    var toastIndex = toastCount++;

    toastr.options = {
        closeButton: true, // $('#closeButton').prop('checked'),
        debug: false, // $('#debugInfo').prop('checked'),
        positionClass: 'toast-bottom-left',// $('#positionGroup input:checked').val() || 'toast-top-right',
        onclick: null
    };


    //if ($showDuration.val().length)
    {
        toastr.options.showDuration = $showDuration;
    }

    //if ($hideDuration.val().length)
    {
        toastr.options.hideDuration = $hideDuration;
    }

    //if ($timeOut.val().length)
    {
        toastr.options.timeOut = $timeOut;
    }

    //if ($extendedTimeOut.val().length)
    {
        toastr.options.extendedTimeOut = $extendedTimeOut;
    }

    //if ($showEasing.val().length) 
    {
        toastr.options.showEasing = $showEasing;
    }

    //if ($hideEasing.val().length)
    {
        toastr.options.hideEasing = $hideEasing;
    }

    //if ($showMethod.val().length) 
    {
        toastr.options.showMethod = $showMethod;
    }

    //if ($hideMethod.val().length)
    {
        toastr.options.hideMethod = $hideMethod;
    }


    $("#toastrOptions").text("Command: toastr[" + shortCutFunction + "](\"" + msg + (title ? "\", \"" + title : '') + "\")\n\ntoastr.options = " + JSON.stringify(toastr.options, null, 2));

    var $toast = toastr[shortCutFunction](msg, title); // Wire up an event handler to a button in the toast, if it exists
    $toastlast = $toast;
}



function refreshGrid(targetId) {
    targetId = targetId.replaceAll('grid_', "").replaceAll('#', "");
    targetId = $("#grid_" + targetId);
    $(targetId).data('kendoGrid').dataSource.read();
    $(targetId).data('kendoGrid').refresh();
}

function refreshListView(targetId) {
    $(targetId).data("kendoListView").dataSource.read();
    $(targetId).data("kendoListView").refresh();
}


function refreshKendoTreeView(targetId) {
    $(targetId).data("kendoTreeView").dataSource.read();
    $(targetId).data("kendoTreeView").refresh();
}

function closeKendoWindow(targetWindowId) {
    $(targetWindowId).data("kendoWindow").close();
}



function expandAndSelectNode(id, treeViewName) {
    // get the Kendo TreeView widget by it's ID given by treeviewName
    var treeView = $(treeViewName).data('kendoTreeView');

    // find node with data-id = id
    var item = $(treeViewName).find("li[data-id='" + id + "']").find(".k-in");

    // expand all parent nodes
    $(item).parentsUntil('.k-treeview').filter('.k-item').each(
        function (index, element) {
            $(treeViewName).data('kendoTreeView').expand($(this));
        }
    );

    // get DataSourceItem by given id
    var nodeDataItem = treeView.dataSource.get(id);
    //get node within treeview widget by uid
    var node = treeView.findByUid(nodeDataItem.uid);

    treeView.select(node);
}





function removeFilter(filter, searchFor) {
    if (filter == null)
        return [];

    for (var x = 0; x < filter.length; x++) {

        if (filter[x].filters != null && filter[x].filters.length >= 0) {
            if (filter[x].filters.length == 0) {
                filter.splice(x, 1);
                return removeFilter(filter, searchFor);
            }
            filter[x].filters = removeFilter(filter[x].filters, searchFor);
        }
        else {
            if (filter[x].field == searchFor) {
                filter.splice(x, 1);
                return removeFilter(filter, searchFor);
            }
        }
    }

    return filter;
}

function kendoGridAddFilter(gridId, fieldName, operator, value) {

    var grid = $(gridId).data("kendoGrid");
    var $filter = new Array();
    if ($(gridId).data('kendoGrid').dataSource.filter() !== undefined && $(gridId).data('kendoGrid').dataSource.filter() !== null) {
        $filter = $(gridId).data('kendoGrid').dataSource.filter().filters;
    }

    $filter = removeFilter($filter, fieldName);

    $filter.push({ field: fieldName, operator: operator, value: value });
    grid.dataSource.filter($filter);
}

function kendoGridBatchFilter(gridId, params) {
    var grid = $(gridId).data("kendoGrid");

    var $filter = new Array();
    if ($(gridId).data('kendoGrid').dataSource.filter() !== undefined && $(gridId).data('kendoGrid').dataSource.filter() !== null) {
        $filter = $(gridId).data('kendoGrid').dataSource.filter().filters;
    }

    $.each(params, function (key, value) {
        $filter = removeFilter($filter, value["fieldName"]);
        if (value["value"] !== "") {
            $filter.push({ field: value["fieldName"], operator: value["operator"], value: value["value"] });
        }
    });

    grid.dataSource.filter($filter);
}

function kendoGridRemoveFilter(gridId, fieldName) {
    var grid = $(gridId).data("kendoGrid");
    if ($(gridId).data('kendoGrid').dataSource.filter() !== undefined && $(gridId).data('kendoGrid').dataSource.filter() !== null) {
        var filters = $(gridId).data('kendoGrid').dataSource.filter().filters;
        filters = removeFilter(filters, fieldName);
        grid.dataSource.filter(filters);
    }
}





// Cascade DropDown plugin @[@
$.fn.cascadingDropDown = function (options) {
    var settings = $.extend({
        sourceId: "",
        selectLabel: "",
        dataUrl: ""
    }, options);

    var currentObj = this;

    $(settings.sourceId).change(function () {
        var selected = $(settings.sourceId + ' :selected').val();
        selected = selected == "" ? 0 : selected;
        if (selected == "") {
            currentObj.empty();
            currentObj.append('<option value="">' + settings.selectLabel + '</option>');
            return;
        }

        $.ajax({
            type: "POST",
            url: settings.dataUrl,
            data: " {'selectedValue':'" + selected + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json"

        }).done(function (data) {
            if (data != null) {
                $(currentObj).empty();
                $(currentObj).append('<option value="">' + settings.selectLabel + '</option>');

                $.each(data.result, function (index, data) {
                    $(currentObj).append('<option value="' + data.Id + '">' + data.Text + '</option>');
                });

                $("#s2id_" + currentObj.attr('Id') + " .select2-chosen").html("انتخاب نمایید");
            }
        }).fail(function (response) {
            if (response.status != 0) {
                alert(response.status + " " + response.statusText);
            }
        });
    });
};


function selectSubMenu(menuTitle, subMenuTitle) {
    $(".page-sidebar-menu *").removeClass("active");
    $(".page-sidebar-menu *").removeClass("open");

    //var ctp = $(".page-sidebar-menu .title:contains('" + menuTitle + "')");
    var ctp = $('.page-sidebar-menu .title').filter(function () {
        return $(this).text() == menuTitle;
    });

    //var ct = $(ctp.closest("ul").find(":contains('" + subMenuTitle + "')"));
    var ct = $(ctp.parent().parent().find("ul li a")).filter(function () {
        return $(this).text().trim() == subMenuTitle;
    });

    ctp.after("<span class='selected'></span>");

    ct.parent().parent().parent().addClass('active');
    ctp.parent().parent().addClass('active')

    ct.parent().addClass('active open');
    ct.parent().parent().slideDown();

    ctp.parent().find(".arrow").addClass("open");
}


function validateFileUpload(e, validExtention, maxSize, valmsgId) {
    var result = true
    var files = e.files;
    //var validExtention = [".jpeg", ".jpg", ".png"];
    $.each(files, function () {
        var isValid = $.inArray(this.extension.toLowerCase(), validExtention);
        $(valmsgId + " .thumbnail .valmsg").remove()
        if (isValid === -1) {
            //alert("فایل انتخابی معتبر نمی باشد");
            $(valmsgId + " .thumbnail").append("<div class='valmsg'><span data-valmsg-replace='true' data-valmsg-for='Title' class='field-validation-error'>فایل انتخابی معتبر نمی باشد</span><div/>")
            e.preventDefault();
            result = false;
        } else if ((this.size / 1024) > maxSize) {
            //alert("حداکثر طول فایل مجاز، " + maxSize + " کیلوبایت می باشد");
            $(valmsgId + " .thumbnail").append("<div class='valmsg'><span data-valmsg-replace='true' data-valmsg-for='Title' class='field-validation-error'>" + "حداکثر طول فایل مجاز، " + maxSize + " کیلوبایت می باشد" + "</span><div/>")
            e.preventDefault();
            result = false;
        }

    });

    return result;
}


function UploaderPreview(e, targetId) {
    var files = e.files;
    $.each(files, function () {

        var reader = new FileReader();
        reader.onload = function (e) {
            $(targetId).attr("src", e.target.result);
        }
        reader.readAsDataURL(this.rawFile);

        $(targetId).removeClass('hidden')

    });

}

function UploaderRemovePreview(e, targetId) {
    $(targetId).attr('src', '');
    $(targetId).addClass('hidden');

    $(targetId).parent().find('.valmsg').remove();
}




window.onload = function () {

    $('form[data-ajax=true]').on('submit', function (e) {
        var formdata = new FormData(this);

        var formId = $(this).attr("id");
        var formAction = $(this).attr("action");
        //var formUpdateTargetId = $(this).attr("data-ajax-update");
        var formOnSuccess = $(this).attr("data-ajax-success");
        var formOnFailure = $(this).attr("data-ajax-failure");

        if ($(this).valid()) {
            $.each($("#" + formId + ' input:file'), function (key, fileInput) {
                for (i = 0; i < fileInput.files.length; i++) {
                    formdata.append(fileInput.files[i].name, fileInput.files[i]);
                }
            });

            $.ajax({
                url: formAction,
                data: formdata,
                cache: false,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (dataofconfirm) {
                    window[formOnSuccess](dataofconfirm);
                },
                error: function (dataofconfirm) {
                    window[formOnFailure](dataofconfirm);
                }
            });
        }


        return false;
    });

}

function clearform(targetFormId) {

    $(':input ', targetFormId)
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('checked')
        .removeAttr('selected');

    $("textarea").val('');


    //$(targetFormId + " input").removeClass(".input-validation-error");
    $(targetFormId + " .field-validation-error").hide();

    $(targetFormId + ' :input[type=hidden]').not(targetFormId + " :input[name*='RequestVerificationToken']").val('');
}





function checkMelliCode(meli_code) {
    if (meli_code.length == 10) {
        if (meli_code == '1111111111' ||
            meli_code == '0000000000' ||
            meli_code == '2222222222' ||
            meli_code == '3333333333' ||
            meli_code == '4444444444' ||
            meli_code == '5555555555' ||
            meli_code == '6666666666' ||
            meli_code == '7777777777' ||
            meli_code == '8888888888' ||
            meli_code == '9999999999') {
            return false;
        }
        c = parseInt(meli_code.charAt(9));
        n = parseInt(meli_code.charAt(0)) * 10 +
            parseInt(meli_code.charAt(1)) * 9 +
            parseInt(meli_code.charAt(2)) * 8 +
            parseInt(meli_code.charAt(3)) * 7 +
            parseInt(meli_code.charAt(4)) * 6 +
            parseInt(meli_code.charAt(5)) * 5 +
            parseInt(meli_code.charAt(6)) * 4 +
            parseInt(meli_code.charAt(7)) * 3 +
            parseInt(meli_code.charAt(8)) * 2;
        r = n - parseInt(n / 11) * 11;
        if ((r == 0 && r == c) || (r == 1 && c == 1) || (r > 1 && c == 11 - r)) {
            return true;
        } else {
            return false;
        }
    } else {
        return false;
    }
}
function SetDropEmpty(element) {
    $("#s2id_" + element + " .select2-chosen").html("انتخاب نمایید");
    $('#' + element).empty();
    var option = $('<option></option>').attr("value", "").text("انتخاب نمایید");
    $('#' + element).append(option);
}

function SetDropIndexToSelectItem(element) {

    $("#" + element).val("انتخاب نمایید");
    $('#select2-chosen-1').html("انتخاب نمایید");
}


function SetSelect2ValueText(dropName, value, text) {
    $('#' + dropName).select2('data', { id: value, text: text });
}

function GenarateRandomString(L) {
    var s = '';
    var randomchar = function () {
        var n = Math.floor(Math.random() * 62);
        if (n < 10) return n; //1-10
        if (n < 36) return String.fromCharCode(n + 55); //A-Z
        return String.fromCharCode(n + 61); //a-z
    }
    while (s.length < L) s += randomchar();
    return s;
}


function OnlyDigit(obj) {
    var id = $(obj).attr('id');
    $("#" + id).keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //$(obj).val('');
            return false;
        }
    });
    $('#' + id).keyup(function (e) {
        moneyCommaSep($(this));
    });
}



var gridName;
var createUrl;
var createParameter;
var formName;
var goToUrl;

function SubmitForm(formDiv) {
    var formdata = new FormData($("#" + formDiv)[0]);
    var formAction = $("#" + formDiv).attr("action");
    var formOnSuccess = $("#" + formDiv).attr("data-ajax-success");
    var formOnFailure = $("#" + formDiv).attr("data-ajax-failure");

    if ($("#" + formDiv).valid()) {
        $.each($('#' + formDiv + ' input:file'), function (key, fileInput) {
            for (i = 0; i < fileInput.files.length; i++) {
                formdata.append(fileInput.files[i].name, fileInput.files[i]);
            }
        });

        $.ajax({
            url: formAction,
            data: formdata,
            cache: false,
            processData: false,
            contentType: false,
            type: 'POST',
            success: function (dataofconfirm) {
                window[formOnSuccess](dataofconfirm);
            },
            error: function (dataofconfirm) {
                window[formOnFailure](dataofconfirm);
            }
        });
    }
}

function ShowDataAlert(validations) {
    var type;
    var msg = "<ul style='list-style-type: none;'>";
    $.each(validations, function (key, value) {
        var tt = "<li>" + value.Message + "</li>";
        msg += tt;
        type = value.Type;
    });
    ShowAlertToast(type, '', msg);
}
function OpenWindowCreate(createUrl, createParameter) {
    var targetForm = "#forms-body";
    if (createParameter != null) {
        $.get(createUrl,
            createParameter,
            function (data) {
                $(targetForm).html(data);
                if ($(".Select2")[0]) {
                    $(".Select2").select2();
                }

            });
    } else {
        $.get(createUrl, null, function (data) {
            $(targetForm).html(data);
            if ($(".Select2")[0]) {
                $(".Select2").select2();
            }

        });
    }
    if (goToUrl !== undefined) {
        window.location.href = goToUrl;
    }

}


function OnSuccess(data) {
    if (data.Data !== undefined) {

        if (data.Data[0].IsSuccessed) {
            if (gridName != null) {
                refreshGrid("#" + gridName);
            }
            if (createParameter !== undefined)
                OpenWindowCreate(createUrl, createParameter);
            else
                OpenWindowCreate(createUrl, null);
        }
        ShowDataAlert(data.Data);


    }

}

function SetDropEmpty(element) {
    $('#' + element).empty();
    var option = $('<option></option>').attr("value", "").text("انتخاب نمایید");
    $('#' + element).append(option);
}
function SetDropEmptyEn(element) {
    $('#' + element).empty();
    var option = $('<option></option>').attr("value", "").text("Please Select");
    $('#' + element).append(option);
}

function OpenWindowEdit(id, editUrl) {
    $.get(editUrl, { id: id }, function (data) {
        $("#forms-body").html(data);
        if ($(".Select2")[0]) {
            $(".Select2").select2();
        }
    });
}

var exportFlag = false;
function exportToExcel(e) {
    var sheet = e.workbook.sheets[0];
    var row = sheet.rows[0];
    var columns = e.sender.columns.filter(a => a.hidden != true);
    for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
        row.cells[cellIndex].background = "#aabbcc";
        var enTitle = columns[cellIndex].headerAttributes.enTitle;
        if (typeof (enTitle) != "undefined") {
            row.cells[cellIndex].value = enTitle;
        }
    }
    if (!exportFlag) {
        for (var i = 0; i < e.sender.columns.length; i++) {
            var col = e.sender.columns[i].attributes.class;
            if (col.includes("hiddenInExcel")) {
                e.sender.hideColumn(i);

            }
        }
        e.preventDefault();
        exportFlag = true;
        setTimeout(function () {
            e.sender.saveAsExcel();

        })

    } else {

        if (exportFlag) {
            for (var i = 0; i < e.sender.columns.length; i++) {
                var col = e.sender.columns[i].attributes.class;
                if (col.includes("hiddenInExcel")) {
                    e.sender.showColumn(i);
                }
            }
        }
        exportFlag = false;
    }
}
var formEn = false;
function sumDataInReport() {
    var measureBaseInfos = document.getElementsByClassName("MeasureBaseInfoId");
    if (measureBaseInfos.length == 0) {
        return "";
    }
    var firstMeasureBaseInfoId = 0
    if (measureBaseInfos.length > 0)
        firstMeasureBaseInfoId = parseInt(measureBaseInfos[0].innerHTML)

    var coundMeasureBaseInfoId = 0;
    var measureBaseInId = 0;
    for (var i = 0; i < measureBaseInfos.length; i++) {
        measureBaseInId = parseInt(measureBaseInfos[i].innerHTML);
        if (firstMeasureBaseInfoId === measureBaseInId) {
            coundMeasureBaseInfoId += 1;
        }
    }

    if (coundMeasureBaseInfoId != measureBaseInfos.length) {

        return " "
    }

    var Amount = document.getElementsByClassName("Amount");
    var vahed = "";
    if (Amount.length > 0) {

        vahed = Amount[Amount.length - 1].innerHTML.split('/')[1];
    }
    var value = 0;
    for (var i = 0; i < Amount.length; i++) {
        var temp = Amount[i].innerHTML.replaceAll(',', '').split('/')[0].trim();
        if (temp != "") {
            value += parseFloat(temp);
        }
    }
    if (typeof vahed === "undefined") {
        return "";
    }
    var titleTotal = "مجموع : ";
    if (formEn) {
        titleTotal = "Total : ";
    }
    return titleTotal + commaSeparateNumber(value) + " " + vahed;
}



function sumDataInReport1() {
    var measureBaseInfos = document.getElementsByClassName("MeasureBaseInfoId1");
    if (measureBaseInfos.length == 0) {
        return "";
    }
    var firstMeasureBaseInfoId = 0
    if (measureBaseInfos.length > 0)
        firstMeasureBaseInfoId = parseInt(measureBaseInfos[0].innerHTML)

    var coundMeasureBaseInfoId = 0;
    var measureBaseInId = 0;
    for (var i = 0; i < measureBaseInfos.length; i++) {
        measureBaseInId = parseInt(measureBaseInfos[i].innerHTML);
        if (firstMeasureBaseInfoId === measureBaseInId) {
            coundMeasureBaseInfoId += 1;
        }
    }

    if (coundMeasureBaseInfoId != measureBaseInfos.length) {

        return " "
    }

    var Amount = document.getElementsByClassName("Amount1");
    var vahed = "";
    if (Amount.length > 0) {

        vahed = Amount[0].innerHTML.split('/')[1];
    }
    var value = 0;
    for (var i = 0; i < Amount.length; i++) {
        var temp = Amount[i].innerHTML.replaceAll(',', '').split('/')[0].trim();
        if (temp != "") {
            value += parseFloat(temp);
        }
    }
    if (typeof vahed === "undefined") {
        return "";
    }
    var titleTotal = "مجموع : ";
    if (formEn) {
        titleTotal = "Total : ";
    }
    return titleTotal + commaSeparateNumber(value) + " " + vahed;
}

function sumDataInReportBasedOnValue() {
    var measureBaseInfos = document.getElementsByClassName("MeasureBaseInfoId");
    if (measureBaseInfos.length === 0) {
        return "";
    }
    var firstMeasureBaseInfoId = 0
    if (measureBaseInfos.length > 0)
        firstMeasureBaseInfoId = parseInt(measureBaseInfos[0].innerHTML)

    var coundMeasureBaseInfoId = 0;
    var measureBaseInId = 0;
    for (var i = 0; i < measureBaseInfos.length; i++) {
        measureBaseInId = parseInt(measureBaseInfos[i].innerHTML);
        if (firstMeasureBaseInfoId === measureBaseInId) {
            coundMeasureBaseInfoId += 1;
        }
    }

    if (coundMeasureBaseInfoId !== measureBaseInfos.length) {

        return " "
    }

    var Amount = document.getElementsByClassName("Amount");
    var vahed = document.getElementsByClassName("Measure")[0].innerHTML;

    var value = 0;
    for (var i = 0; i < Amount.length; i++) {
        var temp = Amount[i].innerHTML.trim();
        console.log(temp);
        if (temp !== "") {
            value += parseFloat(temp);
        }
    }
    if (typeof vahed === "undefined") {
        return "";
    }
    var titleTotal = "مجموع : ";
    if (formEn) {
        titleTotal = "Total : ";
    }
    return titleTotal + commaSeparateNumber(value) + " " + vahed;
}
function checkDate(fromDate, uptoDate, fromTime, upToTime) {
    if (fromTime.length != 0 && upToTime.length != 0) {
        fromDate += " " + fromTime;
        uptoDate += " " + upToTime;
    }

    if (fromDate.length != 0 && uptoDate.length != 0) {
        if (fromDate <= uptoDate) {
            return true;
        }
    } else {//در نظر نگرفتن تاریخ
        return true;
    }
    return false;
}

function checkDateForSingleDate(uptoDate, upToTime) {
    if (upToTime.length !== 0) {
        uptoDate += " " + upToTime;
    }
    return uptoDate.length === 0;

}


var controllerName = "";
function exportToPDF() {
    var data = {};
    if (isExistGetData == true) {
        data = { "request": SetParameter(), "searchedModel": GetData() };
    } else {
        data = { "request": SetParameter() };
    }
    $.post("/" + controllerName + "/SetExcelFilters", data, function (data) {
        window.location.href = "/" + controllerName + "/ReportPdf";
    });
}

function SumReportAmount() {
    $.post("/" + controllerName + "/SumAmountKendoFooter", GetData(), function (data) {
        $('#footerSum').html(data);
    });
    return "";
}

function SumAmount() {
    $.post("/" + controllerName + "/SumAmountKendoFooter", null, function (data) {
        $('#footerSum').html(data);
    });
    return "";
}

function exportToPDF1() {
    var data = {};
    if (isExistGetData == true) {
        data = { "request": SetParameter(), "searchedModel": GetData1() };
    } else {
        data = { "request": SetParameter() };
    }
    $.post("/TruckDailyReport/SetExcelFilters", data, function (data) {
        window.location.href = "/TruckDailyReport/ReportPdf";
    });
}

function exportToPDF2() {
    var data = {};
    if (isExistGetData == true) {
        data = { "request": SetParameter(), "searchedModel": GetData1() };
    } else {
        data = { "request": SetParameter() };
    }
    $.post("/TruckEntryReportOwner/SetExcelFilters", data, function (data) {
        window.location.href = "/TruckEntryReportOwner/ReportPdfDaily";
    });
}
function exportToPDFForGroupCargo() {
    var data = {};
    if (isExistGetDataForGroupCargo === true) {
        data = { "request": SetParameter(), "searchedModel": GetData() };
    } else {
        data = { "request": SetParameter() };
    }
    $.post("/" + controllerName + "/SetExcelFiltersForGroupCargo", data, function (data) {
        window.location.href = "/" + controllerName + "/ReportPdfForGroupCargo";
    });
}
function SetParameter(e) {
    var gn = gridName.replace("grid_", "");
    var grid = $("#grid_" + gn).data("kendoGrid");
    parameterMap = grid.dataSource.transport.parameterMap;
    return parameterMap({
        sort: grid.dataSource.sort(), filter: grid.dataSource.filter(), group: grid.dataSource.group()
    });
    e.preventDefault();
}
var isExistGetData = true;
function exportToExcel() {
    var data = {};
    if (isExistGetData === true) {
        data = { "request": SetParameter(), "searchedModel": GetData() };
    } else {
        data = { "request": SetParameter() };
    }
    $.ajax({
        url: "/" + controllerName + "/SetExcelFilters",
        data: data,
        type: "POST",
        success: function (result) {
            window.location.href = "/" + controllerName + "/ExportExcel";
        }
    });
}
function exportToExcel1() {
    var data = {};
    if (isExistGetData == true) {
        data = { "request": SetParameter(), "searchedModel": GetData1() };
    } else {
        data = { "request": SetParameter() };
    }
    $.ajax({
        url: "/TruckDailyReport/SetExcelFilters",
        data: data,
        type: "POST",
        success: function (result) {
            window.location.href = "/TruckDailyReport/ExportExcel";
        }
    });
}
function exportToExcel2() {
    var data = {};
    if (isExistGetData == true) {
        data = { "request": SetParameter(), "searchedModel": GetData1() };
    } else {
        data = { "request": SetParameter() };
    }
    $.ajax({
        url: "/TruckEntryReportOwner/SetExcelFilters",
        data: data,
        type: "POST",
        success: function (result) {
            window.location.href = "/TruckEntryReportOwner/ExportExcelDaily";
        }
    });
}
var isExistGetDataForGroupCargo = false;
function exportToExcelForGroupCargo() {
    var data = {};
    if (isExistGetDataForGroupCargo == true) {
        data = { "request": SetParameter(), "searchedModel": GetData() };
    } else {
        data = { "request": SetParameter() };
    }
    $.ajax({
        url: "/" + controllerName + "/SetExcelFiltersForGroupCargo",
        data: data,
        type: "POST",
        success: function (result) {
            window.location.href = "/" + controllerName + "/ExportExcelForGroupCargo";
        }
    });
}
function setSlash(input) {

    if (input.value.length == 4) {
        input.value += "/";
    }
    if (input.value.length == 7) {
        input.value += "/";
    }
    if (input.value.length == 10) {

        var month = parseInt(input.value.substring(5, 7));
        var day = parseInt(input.value.substring(8, 10));

        if (month > 12 || month == 0) {
            ShowAlertToast(1, 'خطا!', 'ماه را صحیح وارد نمایید');
            return;
        }
        if (day > 31 || day == 0) {
            ShowAlertToast(1, 'خطا!', 'روز را صحیح وارد نمایید');
            return;
        }
        //$("#" + input.id).change();
    } else if (input.value.length > 10) {
        ShowAlertToast(1, 'خطا!', 'فرمت تاریخ  صحیح نیست');
        return;
    }
}

function DateValidate(input) {

    if (input.value == "")
        return;
    if (input.value.length != 10) {
        ShowAlertToast(1, 'خطا!', 'فرمت تاریخ  صحیح نیست');
        return;
    }
    if (input.value.length == 10) {
        var month = parseInt(input.value.substring(5, 7));
        var day = parseInt(input.value.substring(8, 10));
        var year = parseInt(input.value.substring(0, 4));

        if (month > 12 || month == 0) {
            month = 12;
            // ShowAlertToast(2, 'خطا!', 'ماه را صحیح وارد نمایید');
        }
        if (day > 31 || day == 0) {
            //ShowAlertToast(2, 'خطا!', 'روز را صحیح وارد نمایید');
            //return;
            if (month <= 6)
                day = 31;
            else
                da = 30;
        }
        input.value = year + "/" + ('0' + month).slice(-2) + "/" + ('0' + day).slice(-2);
    }

}

function DocumentDateValidate(input) {
    DateValidate(input);
    var day = parseInt(input.value.substring(8, 10));
    if (day == 31 || day == 30) {

        var month = parseInt(input.value.substring(5, 7));
        var year = parseInt(input.value.substring(0, 4));

        day = 29;
        input.value = year + "/" + ('0' + month).slice(-2) + "/" + ('0' + day).slice(-2);

        ShowAlertToast(2, 'هشدار!', 'برای روز های 30 و 31 نمی توانید سند ثبت کنید');
    }
}

submitForm = (form, func) => {
    try {
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined)
                    if (response.IsSuccess) {
                        func();
                    }
                ShowAlertToast(response.Type, response.Title, response.Message);
            },
            error: function (err) {
                ShowAlertToast(2, '', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
    return false;
}

submitFormNormal = (form) => {
    try {
        if (event != undefined)
            event.preventDefault();
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined)
                    ShowAlertToast(response.Type, response.Title, response.Message);
            },
            error: function (err) {
                ShowAlertToast(2, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, errorInConnectToServer, e);
    }
    return false;
}
submitFormAndGetResult = (form, targetId) => {
    try {
    if (event != undefined)
        event.preventDefault();
    $.ajax({
        type: "POST",
        url: form.action,
        data: new FormData(form),
        contentType: false,
        processData: false,
        success: function (response) {
            if (response != undefined) {
                if (response.Message.length > 0)
                    ShowAlertToast(response.Type, response.Title, response.Message);
                document.getElementById(targetId).innerHTML = response.Data;
                setSelect2AndSwitcheryStyle();
            }
        },
        error: function (err) {
            ShowAlertToast(2, '', err);
        }
    });
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
    return false;
}



//submit form and refresh kendo grid
submitFormByRefreshGrid = (form, targetId, refreshGridId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var gridName = '#' + refreshGridId;
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                    if (response.IsSuccess) {
                        refreshGrid(gridName);
                        document.getElementById(targetId).innerHTML = response.Data;
                    } else {
                        document.getElementById(targetId).innerHTML = response.Data;
                    }
                    setSelect2AndSwitcheryStyle();

                } else {
                    ShowAlertToast(1, 'Error In Load Data!', data);
                }
            },
            error: function (err) {
                ShowAlertToast(1, 'Error In Load Data!', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
    return false;
}
//ShowAlertToast(type, ttl, message)
loadData = (url, targetId, func) => {
    try {
        //event.preventDefault();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById(targetId).innerHTML = "";
                    document.getElementById(targetId).innerHTML = response.Data;
                    setSelect2AndSwitcheryStyle();
                    if (isFunction(func)) {
                        func();
                    }
                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }

            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });

    } catch (e) {
        ShowAlertToast(2, errorInConnectToServer, e);
    }
    return false;
}
loadDataInInput = (url, targetId, func) => {
    try {
        //event.preventDefault();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById(targetId).value = response.Data;
                    setSelect2AndSwitcheryStyle();
                    if (isFunction(func)) {
                        func();
                    }
                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }

            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });

    } catch (e) {
        ShowAlertToast(2, errorInConnectToServer, e);
    }
    return false;
}


loadDataBySelectList = (url, selectListId, targetId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var e = document.getElementById(selectListId);
        var selected_value = e.value;
        $.ajax({
            type: "Get",
            url: url,
            data: { input: selected_value },
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById(targetId).innerHTML = response.Data;
                    setSelect2AndSwitcheryStyle()
                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, 'Error In Load Data!', err);
            }
        });

    } catch (e) {
        ShowAlertToast(2, '', e);
    }
    return false;
}


updateDataAndRefreshGrid = (url, gridId) => {
    try {
        var gridName = '#' + gridId;
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    refreshGrid(gridName);
                } else {
                    ShowAlertToast(data.type, data.title, data.message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, 'Error In Load Data!', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
    return false;
}


loadDataAndRefreshGrid = (url, targetId, gridId) => {
    try {
        var gridName = '#' + gridId;
        $.ajax({
            type: "Get",
            url: url,
            success: function (data) {
                document.getElementById(targetId).innerHTML = data;
                refreshGrid(gridName);
            },
            error: function (err) {
                ShowAlertToast(1, 'Error In Load Data!', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
    return false;
}

loadDataWithMessage = (url, targetId) => {
    try {
        $.ajax({
            type: "Get",
            url: url,
            success: function (data) {
                ShowAlertToast(data.type, data.title, data.message);
                document.getElementById(targetId).innerHTML = data.data
            },
            error: function (err) {
                ShowAlertToast(1, 'Error In Load Data!', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
    return false;
}

loadDataWithMessageAndRefreshGrid = (url, targetId, gridId) => {
    try {
        var gridName = '#' + gridId;
        $.ajax({
            type: "Get",
            url: url,
            success: function (data) {
                ShowAlertToast(data.type, data.title, data.message);
                document.getElementById(targetId).innerHTML = data.data;
                refreshGrid(gridName);
            },
            error: function (err) {
                ShowAlertToast(1, 'Error In Load Data!', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
    return false;
}

function fillSelectList(firstSelectId, targetSelectId, url) {
    try {
        var e = document.getElementById(firstSelectId);
        var selected_value = e.value;
        var selected_text = e.options[e.selectedIndex].text;

        $.ajax({
            type: "Get",
            url: url,
            data: { id: selected_value },
            success: function (response) {
                select = document.getElementById(targetSelectId);
                select.innerHTML = '';
                select.innerHTML = '<option selected value="">انتخاب کنید</option>';
                for (var i = 0; i < response.length; i++) {
                    var opt = document.createElement('option');
                    opt.value = response[i].Value;
                    opt.innerHTML = response[i].Text;
                    select.appendChild(opt);
                }
            },
            error: function (err) {
                ShowAlertToast(1, 'Error In Load Data!', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
}

function fillSelectListBy(targetSelectId, url) {
    try {
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                select = document.getElementById(targetSelectId);
                select.innerHTML = '';
                select.innerHTML = '<option>انتخاب کنید</option>';
                for (var i = 0; i < response.length; i++) {
                    var opt = document.createElement('option');
                    opt.value = response[i].Value;
                    opt.innerHTML = response[i].Text;
                    select.appendChild(opt);
                }
            },
            error: function (err) {
                ShowAlertToast(1, 'Error In Load Data!', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
}


function loadDataInModal(url, targetModalId) {
    try {
        if (event != undefined)
            event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById(modalBodyId).innerHTML = response.Data;
                    $(modalId).modal('show');
                    setSelect2AndSwitcheryStyle()

                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });

    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}

function loadDataInModalWithConfirm(url, targetModalId, confirmText) {
    try {
        if (event != undefined)
            event.preventDefault();
        if (confirm(confirmText) == true) {
            var modalId = '#' + targetModalId;
            var modalBodyId = targetModalId + '_body';
            $.ajax({
                type: "Get",
                url: url,
                success: function (response) {
                    if (response.IsSuccess) {
                        document.getElementById(modalBodyId).innerHTML = response.Data;
                        $(modalId).modal('show');
                        setSelect2AndSwitcheryStyle()
                    } else {
                        ShowAlertToast(response.Type, response.Title, response.Message);
                    }
                },
                error: function (err) {
                    ShowAlertToast(1, errorInProccessDataInServer, err);
                }
            });
        }

    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}

submitFormAndShowModal2 = (form, targetModalId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        form.reset();
                    }
                    document.getElementById(modalBodyId).innerHTML = response.Data;
                    ShowAlertToast(response.Type, response.Title, response.Message);
                    $(modalId).modal('show');
                }
            },
            error: function (err) {
                debugger
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}

submitFormAndShowModal3 = (form, targetModalId, func) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        func();
                    }
                    document.getElementById(modalBodyId).innerHTML = response.Data;
                    ShowAlertToast(response.Type, response.Title, response.Message);
                    $(modalId).modal('show');
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}

submitModalFormAndUpdateTarget = (form, targetId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        document.getElementById(targetId).innerHTML = response.Data;
                        form.reset();
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}

submitFormAndShowResulMessage = (form, targetModalId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                    $(modalId).modal('show');
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}

submitFormAndShowModalWithGetResult = (form, targetModalId, targetId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        document.getElementById(targetId).innerHTML = response.Data;
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                    $(modalId).modal('show');
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}

submitFormAndCloseModalWithGetResult = (form, targetModalId, targetId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        document.getElementById(targetId).innerHTML = response.Data;
                        $(modalId).modal('hide');
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}


submitFormAndCloseModal = (form, targetModalId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        $(modalId).modal('hide');
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}


submitFormAndShowModal = (form, targetModalId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        ShowAlertToast(response.Type, response.Title, response.Message);
                        $(modalId).modal('hide');
                    } else {
                        ShowAlertToast(response.Type, response.Title, response.Message);
                        document.getElementById(modalBodyId).innerHTML = response.Data;
                        $(modalId).modal('show');
                    }
                }
            },
            error: function (err) {
                ShowAlertToast(2, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, errorInConnectToServer, e);
    }
    return false;
}

submitFormByRefreshGridInModal = (form, targetModalId, refreshGridId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        var gridName = '#' + refreshGridId;
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                    if (response.IsSuccess) {
                        refreshGrid(gridName);
                        document.getElementById(modalBodyId).innerHTML = response.Data;
                    } else {
                        document.getElementById(modalBodyId).innerHTML = response.Data;
                    }

                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, errorInConnectToServer, e);
    }
    return false;
}

function getRequestWithConfirm(url, confirmText) {
    try {
        if (confirm(confirmText) == true) {
            $.ajax({
                type: "Get",
                url: url,
                success: function (response) {
                    if (response != undefined) {
                        ShowAlertToast(response.Type, response.Title, response.Message);
                    }
                },
                error: function (err) {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            });
        }

    } catch (e) {
        ShowAlertToast(2, errorInConnectToServer, e);
    }

}

function isFunction(functionToCheck) {
    if (typeof functionToCheck === 'function') {
        return true;
    }
    return functionToCheck && {}.toString.call(functionToCheck) === '[object Function]';
}

function getRequestWithConfirmRefreshGrid(url, confirmText, gridId, func) {
    try {
        if (confirm(confirmText) == true) {
            $.ajax({
                type: "Get",
                url: url,
                success: function (response) {
                    if (response != undefined) {
                        var gridName = '#' + gridId;
                        refreshGrid(gridName);
                        ShowAlertToast(response.Type, response.Title, response.Message);
                        if (isFunction(func)) {
                            func();
                        }
                    }
                },
                error: function (err) {
                    ShowAlertToast(1, errorInProccessDataInServer, err);
                }
            });
        }

    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }

}

function getRequestWithConfirmUpdateTargetElement(url, confirmText, targetId) {
    try {
        if (confirm(confirmText) == true) {
            $.ajax({
                type: "Get",
                url: url,
                success: function (response) {
                    if (response != undefined) {
                        ShowAlertToast(response.Type, response.Title, response.Message);
                        if (response.IsSuccess) {
                            document.getElementById(targetId).innerHTML = response.Data;
                        }
                    }
                },
                error: function (err) {
                    ShowAlertToast(1, errorInProccessDataInServer, err);
                }
            });
        }

    } catch (e) {
        ShowAlertToast(2, errorInConnectToServer, e);
    }

}

function safeOnlyNumber(input, evt) {
    toEnglishDigits(input);
    return onlyNumberKey(evt);
}

function onlyNumberKey(evt) {
    var ASCIICode = (evt.which) ? evt.which : evt.keyCode
    if (ASCIICode > 31 && (ASCIICode < 48 || ASCIICode > 57))
        return false;
    return true;
}

function toEnglishDigits(input) {
    var result = "";
    var persian = { '۰': '0', '۱': '1', '۲': '2', '۳': '3', '۴': '4', '۵': '5', '۶': '6', '۷': '7', '۸': '8', '۹': '9' };
    var arabic = { '٠': '0', '١': '1', '٢': '2', '٣': '3', '٤': '4', '٥': '5', '٦': '6', '٧': '7', '٨': '8', '٩': '9' };
    result = input.val().replace(/[^0-9.]/g, function (w) {
        return persian[w] || arabic[w] || w;
    });
    input.val(result);
};

function maskInput(maskFormat) {
    $('.number').mask(maskFormat);

    /*$(id).mask('00:00');*/
}
function mobileNumber(evt) {
    if (String.fromCharCode(evt.keyCode).match(/[^0-9]/g)) return false
}
function setSelect2AndSwitcheryStyle() {
    setSelect2Style();
    setSwitcheryStyle()
}
function setSelect2Style() {
    try {
        $('select').select2();
    } catch (e) {

    }
}

function setSwitcheryStyle() {
    var elems = document.querySelectorAll('.switchery ');
    elems.forEach(elem => {
        new Switchery(elem);
    });

}

function previewImage(input, imgId) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            var img = $(`#${imgId}`);
            img.attr('src', e.target.result);
            img.show();
        }

        reader.readAsDataURL(input.files[0]);
    }
}

function previewImage(input, imgId, btnDel) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            var img = $(`#${imgId}`);
            img.attr('src', e.target.result);
            img.show();
            $(`#${btnDel}`).show();
        }

        reader.readAsDataURL(input.files[0]);

    }
    $(`#IsDeleteImage`).val('false');
}
function removePreviewImage(imgId, btnDel, fileInput, isDeleteImageId) {
    var img = $(`#${imgId}`);
    img.attr('src', '');
    $(`#${fileInput}`).val('');
    $(btnDel).hide();
    $(img).hide();
    if (document.getElementById(isDeleteImageId) != undefined) {
        $(`#${isDeleteImageId}`).val('true');
    }


}
//$("#imgInp").change(function () {
//    readURL(this);
//});

function getMonthlyProfitPercentageValue() {
    $.ajax({
        type: "Get",
        url: '/Dashboard/BaseData/GetMonthlyProfitPercentageValue',
        success: function (response) {
            if (response != undefined) {
                return response;
            } else {
                ShowAlertToast(1, 'Error In Load Data!', err);
                return 0;
            }
        },
        error: function (err) {
            ShowAlertToast(1, 'Error In Load Data!', err);
        }
    });
}

//-------- start kendo counter ----------
var counter = 1;
function onDataBound(e) {
    counter = 1;
}

function renderNumber(data) {
    return counter++;
}
function renderRecordNumber(data, gridName) {
    var page = parseInt($("#" + gridName).data("kendoGrid").dataSource.page()) - 1;
    var pagesize = $("#" + gridName).data("kendoGrid").dataSource.pageSize();
    return parseInt(renderNumber(data) + (parseInt(page) * parseInt(pagesize)));
}
//-------- end kendo counter ----------


function emptyInupts(...inputIds) {
    for (i = 0; i < inputIds.length; i++) {
        document.getElementById(`${inputIds[i]}`).value = '';
    }
}

function togleShowElement(cheackBox, targetId) {
    if (document.getElementById(targetId) != undefined) {
        if (cheackBox.checked) {
            document.getElementById(targetId).style.display = '';
        } else {
            document.getElementById(targetId).style.display = 'none';
        }
    }
}


//function monyFormat(inputId) {
//    var separator = ",";
//    var int = document.getElementById(inputId).value.replace(new RegExp(separator, "g"), "");
//    var regexp = new RegExp("\\B(\\d{3})(" + separator + "|$)");
//    do {
//        int = int.replace(regexp, separator + "$1");
//    }
//    while (int.search(regexp) >= 0)

//    document.getElementById(inputId).value=int;
//}
function monyFormat(input) {
    var separator = ",";
    var int = input.value.replace(new RegExp(separator, "g"), "");
    var regexp = new RegExp("\\B(\\d{3})(" + separator + "|$)");
    do {
        int = int.replace(regexp, separator + "$1");
    }
    while (int.search(regexp) >= 0)

    input.value = int;
}


function exportPdf() {
    alert(GetData())
}

function openLinkInNewTab(url) {
    window.open(url, '_blank');
}

submitFormAndCloseModal = (form, modalId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        modalId = `#${modalId}`;
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        $(modalId).modal('hide');
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}
submitFormAndCloseModalWitRefreshGrid = (form, modalId, gridName) => {
    try {
        if (event != undefined)
            event.preventDefault();
        modalId = `#${modalId}`;
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        refreshGrid(gridName);
                        $(modalId).modal('hide');
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}


function refreshImage(imgTagId) {
    var src = document.getElementById(imgTagId).src;
    document.getElementById(imgTagId).src = `${src}?` + Date.now();
}

function removeComma(value) {
    if (value != undefined) {
        return value.replace(/,/g, '');
    }
    return "";
}

function getDataFromUrl(url, data, targetId) {
    $.post(`${url}`, data, function (data) {
        $(`#${targetId}`).html(data);
    });
}





function changeInpueState(inputId, value) {
    document.getElementById(inputId).value = value;
}

function openInNewTab(url) {
    window.open(url, "_blank");
}

function exportFile(initDataUrl, data, exportUrl) {
    $.ajax({
        url: initDataUrl,
        cache: false,
        data: data,
        type: 'POST',
        success: function (result) {
            if (result) {
                location.replace(exportUrl);
            } else {
                ShowAlertToast(2, 'امکان خروجی گرفتن فایل نیست');
            }
        },
        error: function (err) {
            ShowAlertToast(1, 'خطا!', 'خطا در دانلود خروجی فایل');
        }
    });
}
function submitByRefreshGrid(form, refreshGridId, func) {

    try {
        if (event != undefined)
            event.preventDefault();
        var gridName = '#' + refreshGridId;
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        if (isFunction(func)) {
                            func();
                        }
                        refreshGrid(gridName);
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                } else {
                    ShowAlertToast(1, errorInProccessDataInServer, data);
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}



//function setPercentageMask() {
//    let input = $('.percent');
//    input.mask('##0.00', { reverse: true });
//    input.bind("change keyup", function () {
//        isBetweenPercentage($(this));
//    });
//}

//function isBetweenPercentage(input) {
//    let myNumber = (input.val()) ? parseFloat(input.val()) : 0;
//    (myNumber.isBetween(0, 100.00)) ? myNumber : input.val('100.00');
//}

//if (typeof (Number.prototype.isBetween) === "undefined") {
//    Number.prototype.isBetween = function (min, max, notBoundaries) {
//        var between = false;
//        if (notBoundaries) {
//            if ((this < max) && (this > min)) between = true;
//        } else {
//            if ((this <= max) && (this >= min)) between = true;
//        }
//        return between;
//    }
//}

//function setPenaltyFactorMask(input) {
//    let input = $('.penaltyFactor');
//    input.mask('##0.00', { reverse: true });
//    input.bind("change keyup", function () {
//        isBetweenPenaltyFactorMask($(this));
//    });
//}

//function isBetweenPenaltyFactorMask(input) {
//    let myNumber = (input.val()) ? parseFloat(input.val()) : 0;
//    (myNumber.isBetween(0, 1.00)) ? myNumber : input.val('1.00');
//}

//if (typeof (Number.prototype.isBetweenInPenaltyFactor) === "undefined") {
//    Number.prototype.isBetweenInPenaltyFactor = function (min, max, notBoundaries) {
//        var between = false;
//        if (notBoundaries) {
//            if ((this < max) && (this > min)) between = true;
//        } else {
//            if ((this <= max) && (this >= min)) between = true;
//        }
//        return between;
//    }
//}


$(document).ready(function () {
    try {
        $('.date').mask('9999/99/99')
    } catch (e) {
        return;
    }
});

function checkAllCheckBoxesByName(elementId, className) {
    if ($(`#${elementId}`).is(':checked')) {
        checkCheckboxes(className);
    } else {
        checkCheckboxes(className, false);
    }
}

function checkCheckboxes(className, isCheck = true) {
    $(`.${className}`).find(':checkbox').each(function () {
        jQuery(this).prop('checked', isCheck);
    });
}

function getCheckBoxesValueInsideElement(className) {
    var selected = [];
    $(`.${className}`).find(':checkbox').each(function () {
        this.checked ? selected.push($(this).val()) : "";
    });
    return selected;
}

function SendMessage(form, confirmMessage = '') {
    try {
        if (confirm(confirmMessage) == true) {
            if (event != undefined)
                event.preventDefault();
            var formData = new FormData(form);
            var customerIds = getCheckBoxesValueInsideElement('customerList');
            for (var i = 0; i < customerIds.length; i++) {
                formData.append('CustomerIds', customerIds[i])
            }
            $.ajax({
                type: "POST",
                url: form.action,
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response != undefined)
                        if (response.IsSuccess) {
                            document.getElementById('MessageContent').value = '';
                            document.getElementById('Title').value = '';
                        }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                },
                error: function (err) {
                    ShowAlertToast(2, '', errorInProccessDataInServer)
                }
            });
        }
    } catch (e) {
        ShowAlertToast(2, '', errorInConnectToServer)
    }
    return false;
}


//$(document).ready(function () {
//    try {
//        $('.doublePrice').mask('999,999,999.99')
//    } catch (e) {
//        return;
//    }
//});

//function doubleMonyFormat(input) {
//    var separator = ",";
//    var int = input.value.replace(new RegExp(separator, "g"), "");
//    var regexp = new RegExp("\\B(\\d{3})(" + separator + "|$)");
//    do {
//        int = int.replace(regexp, separator + "$1");
//    }
//    while (int.search(regexp) >= 0)

//    input.value = int;
//}

function floatNumber(evt, elementId) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    var numberInput = document.getElementById(elementId).value;
    if (charCode !== 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    var dots = numberInput.split('.').length;
    if (dots > 1 && charCode === 46) {
        return false;
    }
    return true;
}



function dotSeparateNumber(val) {
    while (/(\d+)(\d{3})/.test(val.toString())) {
        val = val.toString().replace(/(\d+)(\d{3})/, '$1' + '.' + '$2');
    }
    val = val.toString();
    if (val.includes('.')) {

        var indexDesimal = val.indexOf('.');

        return val.substring(0, indexDesimal + 3);
    } else {
        return val;
    }
}

function validateFloatNumberInput(event) {
    var inputValue = event.target.value;

    var regex = /^[0-9]+(\.[0-9]{0,3})?$/;

    if (!regex.test(inputValue)) {
        event.target.value = inputValue.slice(0, -1);
    }
}


function validateGoldKaratInput(event) {
    var inputValue = event.target.value;
    
    var regex = /^([1-9]|1[0-9]|2[0-4])?$/;
    
    if (!regex.test(inputValue)) {
        event.target.value = inputValue.slice(0, -1);
    }
}