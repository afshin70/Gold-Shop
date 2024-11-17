
submitSocialNetworkFormByRefreshGrid = (form, refreshGridId) => {
    try {
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
                        resetSocialNetworkForm()
                        refreshGrid(gridName);
                    }
                } else {
                    ShowAlertToast(1, 'خطایی در سرور رخ داده است', data);
                }
            },
            error: function (err) {
                ShowAlertToast(1, 'خطا در ارتباط با سرور', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, 'خطا در هنگام ارسال داده به سرور', e);
    }
    return false;
}

function resetSocialNetworkForm() {
    document.getElementById('SocialNetworkId').value = 0;
    document.getElementById('Title').value = '';
    document.getElementById('Url').value = '';
    document.getElementById('inputFile').value = '';
    //document.getElementById('btnImgDel').style.display = 'none';
    document.getElementById('previewImg').style.display = 'none';
    document.getElementById('previewImg').src = '';
}

submitGoldPriceInfoFormByRefreshGrid = (form, refreshGridId) => {
    try {
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
                        form.reset();
                        refreshGrid(gridName);
                    }
                } else {
                    ShowAlertToast(1, 'خطایی در سرور رخ داده است', data);
                }
            },
            error: function (err) {
                ShowAlertToast(1, 'خطا در ارتباط با سرور', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, 'خطا در هنگام ارسال داده به سرور', e);
    }
    return false;
}