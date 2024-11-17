submitConfirmPaymentModalFormAndUpdateTarget = (form, modalId, gridName) => {
    try {
        var modalid = `#${modalId}`;
        var gridId = `#${gridName}`;
        event.preventDefault();
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        //document.getElementById(updateElementId).innerHTML = response.Data;
                        refreshGrid(gridId);
                        $(modalid).modal('hide');
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, 'خطا در محاسبه دوره سررسید اقساط.', response);
            }
        });
    } catch (e) {
        ShowAlertToast(1, 'خطا در ارتباط با سرور', e);
    }
    return false;
}

//function fillPaymentDescription() {

//    try {
//        var paymentType = document.getElementById("PaymentType");
//        var amount = document.getElementById('PaymentAmount').value;
//        var installmentId = document.getElementById('InstallmentId').value;
//        var paymentId = 0;
//        var url = `/Dashboard/Document/GetPaymentDescription?installmentId=${parseInt(installmentId)}&paymentId=${parseInt(paymentId)}&amount=${amount}`;
//        $.ajax({
//            type: "Get",
//            url: url,
//            success: function (response) {
//                if (response != undefined) {
//                    if (document.getElementById('Description') != undefined) {
//                        document.getElementById('Description').value = "";
//                        let selectedPaymentTypeText = paymentType.options[paymentType.selectedIndex].text;
//                        if (selectedPaymentTypeText=="انتخاب کنید") {
//                            selectedPaymentTypeText = "";
//                        }
//                        document.getElementById('Description').value = response + " " + selectedPaymentTypeText ;
//                    }
//                }
//            },
//            error: function (err) {
//                ShowAlertToast(1, errorInProccessDataInServer, err);
//            }
//        });

//    } catch (e) {
//        ShowAlertToast(2, errorInConnectToServer, e);
//    }
//    return false;


//}


function DelayCalculation(delayCalculationUrl, installmentId, selectedPaymentId, autoFillDelayDay = true) {
    try {
        var paymentDate = document.getElementById('PersianPaymentDate').value;
        $.ajax({
            type: "Get",
            url: delayCalculationUrl,
            data: { installmentId: installmentId, paymentDate: paymentDate, selectedPaymentId: selectedPaymentId },
            success: function (response) {
                if (response != undefined && autoFillDelayDay) {
                    document.getElementById('DelayDay').value = response.CurrentInstallmentDelayDay;
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, err);
            }
        });

    } catch (e) {

    }
}
//function fillPaymentDescriptionWithDelayDays(delayCalculationUrl, installmentId, selectedPaymentId, autoFillDelayDay = true) {
//    DelayCalculation(delayCalculationUrl, installmentId, selectedPaymentId, autoFillDelayDay);
//    fillPaymentDescription();
//}

function fillPaymentDescriptionWithDelayDays(autoFillDelayDay = true) {

    try {
        debugger
        var PaymentAmount = document.getElementById('PaymentAmount').value;
        var InstallmentId = document.getElementById('InstallmentId').value;
        var PaymentId = 0; //document.getElementById('PaymentId').value;
        var DelayDay = document.getElementById('DelayDay').value;
        var PaymentDate = document.getElementById('PersianPaymentDate').value;
        var IsCalcWithPaymentDate = autoFillDelayDay;

        var formData = new FormData();
        formData.append('PaymentAmount', PaymentAmount);
        formData.append('InstallmentId', InstallmentId);
        formData.append('PaymentId', PaymentId);
        formData.append('DelayDay', DelayDay);
        formData.append('PaymentDate', PaymentDate);
        formData.append('IsCalcWithPaymentDate', IsCalcWithPaymentDate);

        $.ajax({
            type: "POST",
            url: '/Dashboard/Document/GeneratePaymentInfo',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.IsSuccess) {
                    debugger
                    if (document.getElementById('Description') != undefined) {
                        document.getElementById('Description').value = response.Data.Description;
                    }
                    if (document.getElementById('CustomerMessageContent') != undefined) {
                        document.getElementById('CustomerMessageContent').value = response.Data.Message;
                    }
                    if (document.getElementById('DelayDay') != undefined) {
                        document.getElementById('DelayDay').value = response.Data.DelayDay;
                    }
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

function fillPaymentDescription() {
    try {
        //var paymentType = document.getElementById("PaymentType");
        var amount = document.getElementById('PaymentAmount').value;
        var installmentId = document.getElementById('InstallmentId').value;
        var paymentId = 0;// document.getElementById('PaymentId').value;
        //var paymentDate = document.getElementById('PersianPaymentDate').value;
        var delayDay = document.getElementById('DelayDay').value;
        var url = `/Dashboard/Document/GetPaymentDescription?installmentId=${parseInt(installmentId)}&paymentId=${parseInt(paymentId)}&amount=${amount}&delayDay=${parseInt(delayDay)}`;
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    if (document.getElementById('Description') != undefined) {
                        document.getElementById('Description').value = response.Data.Description;
                    }
                    if (document.getElementById('CustomerMessageContent') != undefined) {
                        document.getElementById('CustomerMessageContent').value = response.Data.Message;
                    }
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
submitRejectionPaymentModalFormAndUpdateTarget = (form, modalId, gridName) => {
    try {
        var modalid = `#${modalId}`;
        var gridId = `#${gridName}`;
        event.preventDefault();
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        refreshGrid(gridId);
                        $(modalid).modal('hide');
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInProccessDataInServer, response);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
    return false;
}