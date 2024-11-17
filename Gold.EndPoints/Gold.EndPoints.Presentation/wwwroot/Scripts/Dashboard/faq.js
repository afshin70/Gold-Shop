

var answerEditor = CKEDITOR.replace('AnswerEditor');
function setCKEditor(htmlData) {
    answerEditor.setData(htmlData);
}
answerEditor.on('change', function (evt) {
    document.getElementById('Answer').value = answerEditor.getData()
});
function initCkEditorInstance() {
    answerEditor = CKEDITOR.replace('AnswerEditor');
}

function resetFAQCategory() {
    document.getElementById('Title').value = '';
    document.getElementById('Id').value = '';
}
function GetData() { return; }
function resetFAQ() {
    document.getElementById('Id').value = '';
    $("#CategoryId").val('').change();
    document.getElementById('Question').value = '';
    document.getElementById('Answer').value = '';
    setCKEditor('')
}

loadFAQData = (url) => {
    try {
        //event.preventDefault();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById('Question').value = response.Data.Question;
                    document.getElementById('Answer').value = response.Data.Answer;
                    document.getElementById('Id').value = response.Data.Id;
                    $("#CategoryId").val(response.Data.CategoryId).change();
                    setCKEditor(response.Data.Answer)
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

function loadListByCategory(gridName) {
    var gridId = `#${fAQGridName}`
    refreshGrid(gridId)
}

//submitSiteContentFormAndGetResult = (form) => {
//    try {
//        if (event != undefined)
//            event.preventDefault();
//        $.ajax({
//            type: "POST",
//            url: form.action,
//            data: new FormData(form),
//            contentType: false,
//            processData: false,
//            success: function (response) {
//                if (response != undefined) {
//                    if (response.Message.length > 0)
//                        ShowAlertToast(response.Type, response.Title, response.Message);

//                }
//            },
//            error: function (err) {
//                ShowAlertToast(2, '', err);
//            }
//        });
//    } catch (e) {
//        ShowAlertToast(2, '', e);
//    }
//    return false;
//}
