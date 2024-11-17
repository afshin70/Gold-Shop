//function initCKEditor(selectorItem)
//{
//    //CKEDITOR.config.baseFloatZIndex = 999999999;
//    CKEDITOR.replace(selectorItem, {
//        language: 'fa',
//        toolbar: [
//    //{ name: 'document', groups: ['mode', 'document', 'doctools'], items: ['Source', '-', 'Save', 'NewPage', 'Preview', 'Print', '-', 'Templates'] },
//    { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
//    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
//    //{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl', 'Language'] },
//    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl'] },
//    //{ name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
//    //{ name: 'colors', items: ['TextColor', 'BGColor'] },
//    { name: 'tools', items: ['Maximize', 'ShowBlocks'] },
//        ],
//        toolbarGroups: [
//    { name: 'document', groups: ['mode', 'document', 'doctools'] },
//    { name: 'clipboard', groups: ['clipboard', 'undo'] },
//    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
//    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
//    { name: 'styles' },
//    { name: 'colors' },
//    { name: 'tools' },
//        ]
//    });
//    CKEDITOR.replace('ContractTextGoodwill', {
//        language: 'fa',
//        toolbar: [
//    { name: 'document', groups: ['mode', 'document', 'doctools'], items: ['Source', '-', 'Save', 'NewPage', 'Preview', 'Print', '-', 'Templates'] },
//    { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
//    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
//    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl', 'Language'] },
//    { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
//    { name: 'colors', items: ['TextColor', 'BGColor'] },
//    { name: 'tools', items: ['Maximize', 'ShowBlocks'] },
//        ],
//        toolbarGroups: [
//    { name: 'document', groups: ['mode', 'document', 'doctools'] },
//    { name: 'clipboard', groups: ['clipboard', 'undo'] },
//    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
//    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
//    { name: 'styles' },
//    { name: 'colors' },
//    { name: 'tools' },
//        ]
//    });
//    CKEDITOR.replace('ContractTextTenant', {
//        language: 'fa',
//        toolbar: [
//    { name: 'document', groups: ['mode', 'document', 'doctools'], items: ['Source', '-', 'Save', 'NewPage', 'Preview', 'Print', '-', 'Templates'] },
//    { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
//    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
//    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl', 'Language'] },
//    { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
//    { name: 'colors', items: ['TextColor', 'BGColor'] },
//    { name: 'tools', items: ['Maximize', 'ShowBlocks'] },
//        ],
//        toolbarGroups: [
//    { name: 'document', groups: ['mode', 'document', 'doctools'] },
//    { name: 'clipboard', groups: ['clipboard', 'undo'] },
//    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
//    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
//    { name: 'styles' },
//    { name: 'colors' },
//    { name: 'tools' },
//        ]
//    });
//}

function initCKEditor(scope)
{
    scope.editorOptions = {
        language: 'fa',
        toolbar: [
    //{ name: 'document', groups: ['mode', 'document', 'doctools'], items: ['Source', '-', 'Save', 'NewPage', 'Preview', 'Print', '-', 'Templates'] },
    { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'] },
    //{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl', 'Language'] },
    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl'] },
    //{ name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
    //{ name: 'colors', items: ['TextColor', 'BGColor'] },
    { name: 'tools', items: ['Maximize', 'ShowBlocks'] },
        ],
        toolbarGroups: [
    { name: 'document', groups: ['mode', 'document', 'doctools'] },
    { name: 'clipboard', groups: ['clipboard', 'undo'] },
    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
    { name: 'styles' },
    { name: 'colors' },
    { name: 'tools' },
        ]
    };
}

function initMap(title)
{
    $('#pageTitleInMap').empty();
    $('#pageTitleInMap').text(title);
}

function GetStringEncoding(input)
{
    if (input === undefined || input === null || input === '' || input === ' ') {
        return null;
    }
    else {
        return encodeURI(input);
    }
}

function GoToReport(url, formId) {
    var reportType = $('#ReportType').val();
    var serialized = $(formId).serialize().replace(/[^&]+=&/g, '').replace(/&[^&]+=$/g, '').replace(/[^&]+=\.?(?:&|$)/g, '');
    serialized = serialized.replace(/%2F/g, '-').replace(new RegExp("/", 'g'), "-");

    var rt = 'ReportType=' + reportType;
    serialized = serialized.replace(('&' + rt), '').replace(rt, '');

    url += ('/' + reportType);
    if (serialized != undefined && serialized != '') {
        url += ('?' + serialized);
    }
    

    window.open(url);
}

function GoToSendSMS(url, formId, smsType) {
    var reportType = $('#ReportType').val();
    var serialized = $(formId).serialize().replace(/[^&]+=&/g, '').replace(/&[^&]+=$/g, '').replace(/[^&]+=\.?(?:&|$)/g, '');
    serialized = serialized.replace(/%2F/g, '-').replace(new RegExp("/", 'g'), "-");

    var rt = 'ReportType=' + reportType;
    serialized = serialized.replace(('&' + rt), '').replace(rt, '');

    url += ('/' + smsType);
    if (serialized != undefined && serialized != '') {
        url += ('?' + serialized);
    }


    window.open(url);
}

function ResetForm() {
    $('#frm-search')[0].reset();
    $("#frm-search").submit();
}

function ShowLoading() {
    $('#loading').fadeIn('normal');
}


function HideLoading() {
    $('#loading').fadeOut('normal');
}

function SendSMS(url) {
    swal({
            title: 'دسترسی غیر فعال',
            text: 'سامانه پیامکی فعالی برای شما در پورتال ثبت نگردیده است. با شماره 09128859496 تماس حاصل فرمایید.',
            html: true,
            confirmButtonText: 'بستن',
            confirmButtonColor: '#52c466',
            type: 'warning'
        });
}