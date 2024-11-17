

var editor = CKEDITOR.replace('TextEditor');
function setCKEditor(htmlData) {
    editor.setData(htmlData);
}

editor.on('change', function (evt) {
    document.getElementById('Text').value = editor.getData()
});

loadSiteContentBySelectList = (url, selectListId) => {
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
                    document.getElementById('Text').innerHTML = response.Data;
                    setCKEditor(response.Data)
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

submitSiteContentFormAndGetResult = (form) => {
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
                    //document.getElementById(targetId).innerHTML = response.Data;
                    //setSelect2AndSwitcheryStyle();
                    //setCKEditor()
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