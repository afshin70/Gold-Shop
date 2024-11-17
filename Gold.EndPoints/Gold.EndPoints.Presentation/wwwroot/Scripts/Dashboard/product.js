function submitCategoryForm(form, refreshGridId) {

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
                        resetCategory();
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

function resetCategory() {
    document.getElementById('Id').value = '';
    document.getElementById('Title').value = '';
}

function loadCategoryData(url) {
    try {
        //event.preventDefault();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById('Id').value = response.Data.Id;
                    document.getElementById('Title').value = response.Data.Title;
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

//product methods
var descriptionEditor = CKEDITOR.replace('DescriptionEditor');
function setCKEditor(htmlData) {
    descriptionEditor.setData(htmlData);
}
descriptionEditor.on('change', function (evt) {
    document.getElementById('Description').value = descriptionEditor.getData()
});
function initCkEditorInstance() {
    descriptionEditor = CKEDITOR.replace('DescriptionEditor');
}

loadProductData = (url) => {
    try {
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById('ProductId').value = response.Data.ProductId;
                    document.getElementById('ProductTitle').value = response.Data.ProductTitle;
                    document.getElementById('Weight').value = response.Data.Weight;
                    document.getElementById('Karat').value = response.Data.Karat;
                    document.getElementById('Wage').value = response.Data.Wage;
                    document.getElementById('GalleryProfit').value = response.Data.GalleryProfit;
                    $("#GalleryId").val(response.Data.GalleryId).change();
                    $("#Status").val(response.Data.StatusStr).change();

                    $("#CategoryIds").val('').change();
                    if (response.Data.CategoryIds != null) {
                        for (var i = 0; i < response.Data.CategoryIds.length; i++) {
                            $("#CategoryIds option[value='" + response.Data.CategoryIds[i] + "']").prop("selected", true);
                        }
                    }
                    setSelect2Style();
                    document.getElementById('StonPrice').value = response.Data.StonPrice;

                    if (response.Data.Description == null) {
                        response.Data.Description = '';
                    }

                    document.getElementById('Description').value = response.Data.Description;
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

function resetProductForm() {
    document.getElementById('ProductId').value = '';
    document.getElementById('ProductTitle').value = '';
    document.getElementById('Weight').value = '';
    document.getElementById('Karat').value = '';
    document.getElementById('Wage').value = '';
    document.getElementById('GalleryProfit').value = '';
    $("#GalleryId").val('').change();
    $("#Status").val('').change();
    $("#CategoryIds").val('').change();
    document.getElementById('StonPrice').value = '0';
    document.getElementById('Description').value = '';
    setCKEditor('');
}


submitProductGalleryForm = (form) => {
    try {
        if (event != undefined)
            event.preventDefault();
        //var modalId = '#' + targetModalId;
        var listId = 'table_productGalleryList';
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
                    }
                    document.getElementById(listId).innerHTML = response.Data;
                    ShowAlertToast(response.Type, response.Title, response.Message);
                    //$(modalId).modal('show');
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

submitProductGalleryForm = (form, targetModalId) => {
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
                        document.getElementById(modalBodyId).innerHTML = response.Data;
                        form.reset();
                    }
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