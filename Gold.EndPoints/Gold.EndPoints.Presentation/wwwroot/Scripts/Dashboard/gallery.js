


var purchaseDescriptionEditor = CKEDITOR.replace('PurchaseDescriptionEditor');
function setCKEditor(htmlData) {
    purchaseDescriptionEditor.setData(htmlData);
}
purchaseDescriptionEditor.on('change', function (evt) {
    document.getElementById('PurchaseDescription').value = purchaseDescriptionEditor.getData()
});
function initCkEditorInstance() {
    purchaseDescriptionEditor = CKEDITOR.replace('PurchaseDescriptionEditor');
}
function resetGalleryForm() {
    document.getElementById('Id').value = 0;
    document.getElementById('Name').value = '';
    document.getElementById('Tel').value = '';
    document.getElementById('IsActive').checked = true;
    document.getElementById('Address').value = '';
    document.getElementById('PurchaseDescription').value = '';
    purchaseDescriptionEditor.setData('');
}
loadGalleryData = (url) => {
    try {
        //event.preventDefault();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById('Id').value = response.Data.Id;
                    document.getElementById('Name').value = response.Data.Name;
                    document.getElementById('Tel').value = response.Data.Tel;
                    document.getElementById('IsActive').checked = response.Data.IsActive;
                    document.getElementById('Address').value = response.Data.Address;
                    document.getElementById('PurchaseDescription').value = response.Data.PurchaseDescription;
                    setCKEditor(response.Data.PurchaseDescription)
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
//function setCKEditor() {
//    if (CKEDITOR.instances['PurchaseDescription']) {
//        delete CKEDITOR.instances['PurchaseDescription'];
//    }
//    CKEDITOR.replace('PurchaseDescription', {
//    });
//}

