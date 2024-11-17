
submitSellerFormByRefreshGrid = (form, refreshGridId) => {
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
                        resetSellerForm();
                        refreshGrid(gridName);
                    }
                    setSelect2AndSwitcheryStyle()

                } else {
                    ShowAlertToast(1, 'خطایی در سرور رخ داده است', data);
                }
            },
            error: function (err) {
                ShowAlertToast(1, 'خطا در ارتباط با سرور', err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
    return false;
}

function resetSellerForm() {
    document.getElementById('SellerId').value = '0';
    document.getElementById('FullName').value = '';
    document.getElementById('Password').value = '';
    document.getElementById('Mobile').value = '';
    document.getElementById('UserName').value = '';
    document.getElementById('ConfirmPassword').value = '';
    document.getElementById('ProductRegisterPerHourCount').value = '';
    if (document.getElementById('alertDialog')!=undefined) {
        document.getElementById('alertDialog').remove();
    }
    document.getElementById('inputFile').value = '';
    document.getElementById('IsDeleteImage').value = 'false';
    document.getElementById('previewImg').src = '';
    document.getElementById('HasAccessToRegisterLoan').checked = false;
    document.getElementById('IsActive').checked = true;
    document.getElementById('HasAccessToRegisterProduct').checked = false;
    document.getElementById('btnImgDel').style.display = 'none';
    // $("#GalleryId").val("-1").change();
    $('#GalleryId').val("-1").change();
}