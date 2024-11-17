submitManagerUserFormByRefreshGrid = (form, refreshGridId) => {
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
                        resetManagerUserForm();
                        refreshGrid(gridName);
                    }
                    //setSelect2AndSwitcheryStyle()
                } else {
                    ShowAlertToast(1, 'خطایی در سرور رخ داده است', data);
                }
            },
            error: function (err) {
                ShowAlertToast(1, 'خطا در ارتباط با سرور', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, 'خطایی هنگام ارسال داده ها به سرور رخ داده است', e);
    }
    return false;
}

function resetManagerUserForm() {
    document.getElementById('ManagerUserId').value = '0';
    document.getElementById('FullName').value = '';
    document.getElementById('Password').value = '';
    document.getElementById('Mobile').value = '';
    document.getElementById('UserName').value = '';
    document.getElementById('ConfirmPassword').value = '';
    document.getElementById('IsActive').checked = true;
    $('#AccessLevelId').val("-1").change();
}