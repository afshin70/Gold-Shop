$(document).ready(function () {
    resetArticleForm();
})

var answerEditor = CKEDITOR.replace('DescriptionEditor');
function setCKEditor(htmlData) {
    answerEditor.setData(htmlData);
}   
answerEditor.on('change', function (evt) {
    document.getElementById('Description').value = answerEditor.getData()
});
function initCkEditorInstance() {
    answerEditor = CKEDITOR.replace('DescriptionEditor');
}

function resetArticleForm() {
    document.getElementById('Id').value = '';
    document.getElementById('Title').value = '';
    //document.getElementById('IsDeleteImageFile').value ='';
    //document.getElementById('IsDeleteVideoFile').value = '';

    //document.getElementById('previewImg').src = '';
    //document.getElementById('previewVideo').src ='';

    document.getElementById('Description').value = '';
    setCKEditor('')
}
function GetData() { return; }

loadArticleData = (url) => {
    try {
        //event.preventDefault();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById('Id').value = response.Data.Id;
                    document.getElementById('Title').value = response.Data.Title;

                    document.getElementById('Description').value = response.Data.Description;
                    //$('#Status').prop('checked', response.Data.Status);
                    //setSelect2AndSwitcheryStyle();

                    setCKEditor(response.Data.Description)
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



submitChangeArticleImageOrVideoForm = (form, imageOrVideoTag) => {
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
                    //debugger
                    if (response.IsSuccess) {
                        form.reset();
                        let html = '';
                        document.getElementById(imageOrVideoTag).innerHTML = html;
                        if (response.Data.IsVideo) {
                            html = ` <video width="640" height="480" controls>
                                                <source id="fileSrc" src="/Dashboard/Article/GetArticleVideo?videoName=${response.Data.FileName}" type="video/mp4">
                                          </video>`;
                        } else {
                            html = ` <img id="fileSrc" style="max-width:500px;" src="/Dashboard/Article/GetArticleImage?imageName=${response.Data.FileName}" />`;
                        }
                        document.getElementById(imageOrVideoTag).innerHTML = html;
                        document.getElementById('btnDeleteFile').style.display = '';
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                    //$(modalId).modal('show');
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
removeArticleImageOrVideoForm = (confirmText,isVideo, id) => {
    try {
        if (event != undefined)
            event.preventDefault();
        if (confirm(confirmText) == true) {
            var form = new FormData();
            form.append('id', id);
            form.append('isVideo', isVideo);
            $.ajax({
                type: "POST",
                url: '/Dashboard/Article/RemoveArticleImageOrVideo',
                data: form,
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response != undefined) {
                        //debugger
                        if (response.IsSuccess) {
                            document.getElementById('videoOrImage').innerHTML = '';
                            document.getElementById('btnDeleteFile').style.display = 'none';
                        }
                        ShowAlertToast(response.Type, response.Title, response.Message);
                        //$(modalId).modal('show');
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