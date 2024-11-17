function emptyEssentialTelForm() {
    document.getElementById('EssentialTelId').value = '0';
    document.getElementById('Tel').value = '';
    document.getElementById('RelationShip').value = '';
}

function emptyCardNumberForm() {
    document.getElementById('CardNumberId').value = '0';
    document.getElementById('CardNumber').value = '';
    document.getElementById('Owner').value = '';
}

function resetCustomerForm() {
    loadData('/Dashboard/Customer/CreateOrEditCustomer?id=0', 'customerForm')
}
submitCustomerFormByRefreshGrid = (form, refreshGridId, func, confirmText) => {
    try {
        if (event != undefined)
            event.preventDefault();
        //check mobile number
        var mobile = document.getElementById("Mobile").value;
        var id = document.getElementById("Id").value;
        $.ajax({
            type: "Get",
            url: "/Dashboard/Customer/IsDuplicateCustomerMobile",
            data: { id: id, mobile: mobile },
            success: function (response) {
                if (response != undefined) {
                    if (response) {
                        if (confirm(confirmText) == true) {
                            submitByRefreshGrid(form, refreshGridId, func)
                        }
                    } else {
                        submitByRefreshGrid(form, refreshGridId, func)
                    }
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

function getAccountStatus(url, customerId) {
    var s = 1;
    let title = s;
    $.ajax({
        url: url,
        cache: false,
        type: 'GET',
        data: { customerId: customerId },
        success: function (result) {
            title = result.Data;
        },
        error: function (err) {
            console.log(err);
            title = err;
        }
    });
    s = s + 1;
    return title;
}


function changeSearchType(searchType) {
    loadData(`/Dashboard/Customer/SearchInCustomers?searchType=${searchType}`, 'searchForm')
    var searchByPhoneNumberSection = document.getElementById('searchByPhoneNumberSection');
    var searchByBankCardNumberSection = document.getElementById('searchByBankCardNumberSection');
    if (searchByPhoneNumberSection != undefined && searchByBankCardNumberSection != undefined) {
        if (searchType == 'ByPhoneNumber') {
            searchByPhoneNumberSection.style.display = '';
            searchByBankCardNumberSection.style.display = 'none';
        } else if (searchType == 'ByBankCardNumber') {
            searchByPhoneNumberSection.style.display = 'none';
            searchByBankCardNumberSection.style.display = '';
        }
    }

}


function loadDataInBankCardNumberGrid() {
    if (document.getElementById('BankCardNumber') != undefined) {
        let bankCardNumber =document.getElementById('BankCardNumber').value;
        if (bankCardNumber.length >= 4)
            refreshGrid('grid_customersByBankCardNumberGrid');
        else
            ShowAlertToast(1, 'خطا', 'طول شماره کارت بانکی باید حداقل 4 کاراکتر باشد.');
    }
}

function loadDataInPhoneNumberGrid() {
    if (document.getElementById('PhoneNumber') != undefined) {
        let phoneNumber = document.getElementById('PhoneNumber').value;
        if (phoneNumber.length >= 4)
            refreshGrid('grid_customersByPhoneNumberGrid')
        else
            ShowAlertToast(1, 'خطا', 'طول شماره تلفن باید حداقل 4 کاراکتر باشد.');
    }
}