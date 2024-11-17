
let errorInProccessDataInServer = 'خطایی در سرور رخ داده است';
let errorInConnectToServer = 'خطا در ارتباط با سرور';
function refreshImage(imgTagId) {
    var src = document.getElementById(imgTagId).src;
    document.getElementById(imgTagId).src = `${src}?` + Date.now();
}

$(document).ready(function () {
  
    $("#otp input").on("keydown", function (e) {
        // console.log("tes")
        let value = $(this).val();
        let key = e.keyCode;
        if (key == 8) {
            $(this).val("");
            if (value == "")
                $(this).prev().focus();
        }
        if (value.length > 0) {
            // $(this).val(value);
            e.preventDefault();
            return;
        }
    })

    $("#otp input").on("input", function (e) {
        let value = $(this).val();
        if (value.length > 0) {
            $(this).next().focus();
            // $(this).val(value) 
        }

    })
    $('.card-number').mask('0000-0000-0000-0000');
})

function moneyFormat(inputValue = '') {
    var separator = ",";
    var int = inputValue.replace(new RegExp(separator, "g"), "");
    var regexp = new RegExp("\\B(\\d{3})(" + separator + "|$)");
    do {
        int = int.replace(regexp, separator + "$1");
    }
    while (int.search(regexp) >= 0)

    return int;
}

function safeOnlyNumber(input, evt) {
    toEnglishDigits(input);
    return onlyNumberKey(evt);
}

$(".money-format").keyup(function (e) {

    let money = moneyFormat($(this).val());
    $(this).val(money)
});


function setTimer(second = '10') {
    document.getElementById("timer").style.display = 'inline-block'
    document.getElementById("try_again").style.display = 'none'
    var minute = 00;
    var sec = second;
    const timer = setInterval(function () {
        document.getElementById("timer").innerHTML = (minute + ":" + sec);
        sec = parseInt(sec) - 1;
        // sec--;

        if (sec < 00) {
            minute = parseInt(minute) - 1;

            if (minute < 0) {
                clearInterval(timer);
                document.getElementById("timer").style.display = 'none'
                document.getElementById("try_again").style.display = 'inline-block'
                // minute = 5;
            }
            else {
                if (minute < 10)
                    minute = '0' + minute;
                sec = 60;
            }


        }
        else {
            if (sec < 10)
                sec = '0' + sec;
        }
    }, 1000);
}



function fillSelectList(firstSelectId, targetSelectId, url) {

    try {
        var e = document.getElementById(firstSelectId);
        var selected_value = e.value;
        var selected_text = e.options[e.selectedIndex].text;
        $.ajax({
            type: "Get",
            url: url,
            data: { id: selected_value },
            success: function (response) {
                select = document.getElementById(targetSelectId);
                select.innerHTML = '';
                select.innerHTML = '<option selected value="">انتخاب کنید</option>';
                for (var i = 0; i < response.length; i++) {
                    var opt = document.createElement('option');
                    opt.value = response[i].Value;
                    opt.innerHTML = response[i].Text;
                    select.appendChild(opt);
                }
            },
            error: function (err) {
                ShowToastAlert(1, errorInProccessDataInServer)
            }
        });
    } catch (e) {
        ShowToastAlert(2, errorInConnectToServer)
    }
}

function ShowToastAlert(type, message) {
    if (type == 0) {
        Toast.fire({
            icon: 'success',
            title: message
        })
    } else if (type == 1) {
        Toast.fire({
            icon: 'error',
            title: message
        })
    } else if (type == 2) {
        Toast.fire({
            icon: 'warning',
            title: message
        })
    } else if (type == 3) {
        Toast.fire({
            icon: 'info',
            title: message
        })
    }
}

submitFormNormal = (form) => {
    try {
        event.preventDefault();
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined)
                    ShowToastAlert(response.Type, response.Message)
            },
            error: function (err) {
                ShowToastAlert(2, errorInProccessDataInServer);
            }
        });
    } catch (e) {
        ShowToastAlert(2, errorInConnectToServer);
    }
    return false;
}

resendVerifyCode = () => {
    try {
        event.preventDefault();
        showLoader();
        var mobile = document.getElementById('Mobile').value;
        var userName = document.getElementById('UserName').value;
        $.ajax({
            type: "POST",
            url: '/ResendVerifyCode',
            data: { userName: userName, mobile: mobile },
            success: function (response) {
                hideLoader();
                if (response != undefined) {
                    document.getElementById('Token').value = response.Data.Token
                    var timerSeconds = response.Data.Timer.split(':')[2].split('.')[0];
                    setTimer(`${timerSeconds}`);
                    ShowToastAlert(response.Type, response.Message)
                }
            },
            error: function (err) {
                ShowToastAlert(2, errorInProccessDataInServer);
            }
        });
    } catch (e) {
        ShowToastAlert(2, errorInConnectToServer);
    }
}
resendRegisterVerifyCode = () => {
    try {
        event.preventDefault();
        showLoader();
        var mobile = document.getElementById('Mobile').value;
        var nationalCode = document.getElementById('NationalCode').value;
        var nationalityType = document.getElementById('NationalityType').value;
        debugger
        $.ajax({
            type: "POST",
            url: '/ResendRegisterVerifyCode',
            data: { nationalityType: nationalityType, userName: nationalCode, mobile: mobile },
            success: function (response) {
                hideLoader();
                if (response != undefined) {
                    document.getElementById('Token').value = response.Data.Token
                    var timerSeconds = response.Data.Timer.split(':')[2].split('.')[0];
                    setTimer(`${timerSeconds}`);
                    ShowToastAlert(response.Type, response.Message)
                }
            },
            error: function (err) {
                ShowToastAlert(2, errorInProccessDataInServer);
            }
        });
    } catch (e) {
        ShowToastAlert(2, errorInConnectToServer);
    }
}

function validateMobileNumberFormat(element) {
    try {
        var regex = new RegExp("^(\\+98|0)?9\\d{9}$");
        if (regex.test(element.value)) {
            return;
        } else {
            ShowToastAlert(2, 'فرمت شماره موبایل اشتباه میباشد.');
        }
    } catch (e) {
        ShowToastAlert(2, 'فرمت شماره موبایل اشتباه میباشد.');
    }
}

//loader

document.onreadystatechange = function () {
    var state = document.readyState;
    if (state == 'interactive') {
        //setTimeout(function () {
        //    hideLoader();
        //}, 5000);
    } else if (state == 'complete') {
        hideLoader();
    }
}

function showLoader() {
    if (document.getElementById('loader') != undefined) {
        document.getElementById('loader').style.display = 'flex';
    }
}
function hideLoader() {
    if (document.getElementById('loader') != undefined) {
        document.getElementById('loader').style.display = 'none';
    }
}

function getAllPayments(documentId) {
    showLoader();
    $.ajax({
        type: "Get",
        url: '/Document/AllPayments',
        data: { documentId: documentId },
        success: function (response) {
            if (response.IsSuccess) {
                hideLoader();
                document.getElementById('allPayments').innerHTML = response.Data;
            } else {
                ShowToastAlert(response.Type, response.Message)
            }
        },
        error: function (err) {
            ShowToastAlert(1, errorInProccessDataInServer);
        },
        done: hideLoader()
    });
}

function submitCustomerPayment(form, documentId, confirmMessage = '') {
    try {
        event.preventDefault();
        isDisableElement('submitPayment', true)
        if (confirm(confirmMessage) == true) {
            $.ajax({
                type: "POST",
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response != undefined) {
                        ShowToastAlert(response.Type, response.Message)
                        if (response.IsSuccess) {
                            form.reset();
                            if (document.getElementById('file-name') != undefined)
                                document.getElementById('file-name').innerHTML = '';
                            getAllPayments(documentId)
                        }
                    } else {
                        ShowToastAlert(1, errorInProccessDataInServer);
                    }
                },
                error: function (err) {
                    ShowToastAlert(1, errorInProccessDataInServer);
                }
            });
        } else {
            isDisableElement('submitPayment', false)
            return false;
        }
        isDisableElement('submitPayment', false)

    } catch (e) {
        isDisableElement('submitPayment', false)
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}

function isDisableElement(elementId, isDisable = true) {
    if (document.getElementById(elementId) != undefined) {
        document.getElementById(elementId).disabled = isDisable;
    }
}

function submitCustomerProfile(form) {
    try {
        event.preventDefault();
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    //document.getElementById("editProfileInfoForm").reset();
                    refreshImage("profileImage");
                    refreshImage("header_profile_img");
                    // refreshImage("header_profile_img_mobile");
                    ShowToastAlert(response.Type, response.Message)
                } else {
                    ShowToastAlert(1, errorInProccessDataInServer);
                }
            },
            error: function (err) {
                ShowToastAlert(1, errorInProccessDataInServer);
            }
        });
    } catch (e) {
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}

function refreshImage(imgId) {
    document.getElementById(imgId).src += `?v=${new Date().getTime()}`;
    if (document.getElementById('Captcha')!=undefined) {
        document.getElementById('Captcha').value = '';
    }
}

function login(form) {
    try {
        event.preventDefault();
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    ShowToastAlert(response.Type, response.Message)
                    if (response.IsSuccess) {
                        window.location.href = response.Data;
                    } else {
                        if (response.Data == 'False') {
                            if (document.getElementById('Captcha') != undefined)
                                document.getElementById('Captcha').value = '';
                            refreshImage('captcha');
                        }
                    }
                } else {
                    ShowToastAlert(1, errorInProccessDataInServer);
                }
            },
            error: function (err) {
                ShowToastAlert(1, errorInProccessDataInServer);
            }
        });
    } catch (e) {
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}
function submitFormNormal(form) {
    try {
        event.preventDefault();
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    ShowToastAlert(response.Type, response.Message)
                    if (response.IsSuccess) {
                        form.reset();
                    }
                } else {
                    ShowToastAlert(1, errorInProccessDataInServer);
                }
            },
            error: function (err) {
                ShowToastAlert(1, errorInProccessDataInServer);
            }
        });
    } catch (e) {
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}
function submitFormNormalWithConfirm(form, confirmMessage) {
    try {
        event.preventDefault();
        if (confirm(confirmMessage) == true) {
            $.ajax({
                type: "POST",
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response != undefined) {
                        ShowToastAlert(response.Type, response.Message)
                        if (response.IsSuccess) {
                            form.reset();
                        }
                    } else {
                        ShowToastAlert(1, errorInProccessDataInServer);
                    }
                },
                error: function (err) {
                    ShowToastAlert(1, errorInProccessDataInServer);
                }
            });
        } else {
            return false;
        }

    } catch (e) {
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}
submitModalFormEditInformetionWithConfirm = (form, targetModalId, confirmMessage) => {
    try {
        event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        if (confirm(confirmMessage) == true) {
            $.ajax({
                type: "POST",
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response != undefined) {
                        document.getElementById(modalBodyId).innerHTML = response.Data;
                        ShowToastAlert(response.Type, response.Message)
                        if (response.IsSuccess) {
                            $(modalId).modal('hide');
                        }
                    }
                },
                error: function (err) {
                    ShowToastAlert(1, errorInProccessDataInServer);
                }
            });
        }
    } catch (e) {
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}

addNewBankCardNumber = (form, targetModalId, confirmMessage) => {
    try {
        event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        if (confirm(confirmMessage) == true) {
            $.ajax({
                type: "POST",
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response != undefined) {
                        ShowToastAlert(response.Type, response.Message)
                        if (response.IsSuccess) {
                            document.getElementById(modalBodyId).innerHTML = response.Data;
                            document.getElementById('CardNumber').value = '';
                            document.getElementById('Owner').value = '';
                        }
                    }
                },
                error: function (err) {
                    ShowToastAlert(1, errorInProccessDataInServer);
                }
            });
        }
    } catch (e) {
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}
function maskInput(maskFormat) {
    $('.number').mask(maskFormat);
}

function loadDataInModal(url, targetModalId) {
    try {
        if (event != undefined)
            event.preventDefault();

        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById(modalBodyId).innerHTML = response.Data;
                    $(modalId).modal('show');
                    $('.card-number').mask('0000-0000-0000-0000');
                } else {
                    ShowToastAlert(response.Type, response.Message)
                }
            },
            error: function (err) {
                ShowToastAlert(1, errorInProccessDataInServer);
            }
        });
    } catch (e) {
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}


function uploadFile(target) {
    document.getElementById("file-name").innerHTML = target.files[0].name;
}

function safeOnlyNumber(input, evt) {
    toEnglishDigits(input);
    return onlyNumberKey(evt);
}

function onlyNumberKey(evt) {
    var ASCIICode = (evt.which) ? evt.which : evt.keyCode
    if (ASCIICode > 31 && (ASCIICode < 48 || ASCIICode > 57))
        return false;
    return true;
}

function toEnglishDigits(input) {
    var result = "";
    var persian = { '۰': '0', '۱': '1', '۲': '2', '۳': '3', '۴': '4', '۵': '5', '۶': '6', '۷': '7', '۸': '8', '۹': '9' };
    var arabic = { '٠': '0', '١': '1', '٢': '2', '٣': '3', '٤': '4', '٥': '5', '٦': '6', '٧': '7', '٨': '8', '٩': '9' };
    result = input.val().replace(/[^0-9.]/g, function (w) {
        return persian[w] || arabic[w] || w;
    });
    input.val(result);
};

function ShowPassword(passwordInputId, button) {
    // console.log(e.classList)
    var paswordInput = document.getElementById(passwordInputId);

    if (paswordInput.type === "password") {
        paswordInput.type = "text";
        button.innerHTML = '<i class="fa fa-eye-slash"></i>';
    } else {
        paswordInput.type = "password";
        button.innerHTML = '<i class="fa fa-eye"></i>';
    }

}

$(document).ready(function () {
    $('.date').mask('9999/99/99');
    $('form').on('focus', 'input[type=number]', function (e) {
        $(this).on('wheel.disableScroll', function (e) {
            e.preventDefault()
        })
    })
    $('form').on('blur', 'input[type=number]', function (e) {
        $(this).off('wheel.disableScroll')
    })
});


function loadDataInModal2(url, targetModalId) {
    //try {
    if (event != undefined)
        event.preventDefault();
    var modalId = '#' + targetModalId;
    $.ajax({
        type: "Get",
        url: url,
        success: function (response) {
            if (response.IsSuccess) {
                document.getElementById('documentModalContent').innerHTML = response.Data;
                $(modalId).modal('show');
            } else {
                ShowToastAlert(response.Type, response.Message)
            }
        },
        error: function (err) {
            ShowToastAlert(1, errorInProccessDataInServer);
        }
    });
    //} catch (e) {
    //    ShowToastAlert(1, errorInConnectToServer);
    //}
    return false;
}

function isFunction(functionToCheck) {
    if (typeof functionToCheck === 'function') {
        return true;
    }
    return functionToCheck && {}.toString.call(functionToCheck) === '[object Function]';
}


function loadGoldPriceInfoInModal(url, targetModalId) {
    try {
        if (event != undefined)
            event.preventDefault();

        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById(modalBodyId).innerHTML = response.Data;
                    $(modalId).modal('show');
                    setGodPriceTimer()
                } else {
                    ShowToastAlert(response.Type, response.Message)
                }
            },
            error: function (err) {
                ShowToastAlert(1, errorInProccessDataInServer);
            }
        });
    } catch (e) {
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}
function loadGoldPriceCalcModal(url, targetModalId) {
    try {
        if (event != undefined)
            event.preventDefault();

        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById(modalBodyId).innerHTML = response.Data;
                    $(modalId).modal('show');
                    setGodPriceTimer()
                } else {
                    ShowToastAlert(response.Type, response.Message)
                }
            },
            error: function (err) {
                ShowToastAlert(1, errorInProccessDataInServer);
            }
        });
    } catch (e) {
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}

function validatecalculateGoldPriceForm() {
    if (document.getElementById('GramsGoldPrice').value == '') {
        document.getElementById('GramsGoldPriceMessage').innerText = 'اجباری می باشد';
        return false;
    } else {
        if (document.getElementById('GramsGoldPriceMessage') != null)
            document.getElementById('GramsGoldPriceMessage').innerText = '';
    }
    if (document.getElementById('Weight').value == '') {
        document.getElementById('WeightMessage').innerText = 'اجباری می باشد';
        return false;
    } else {
        if (document.getElementById('WeightMessage') != null)
            document.getElementById('WeightMessage').innerText = '';
    }
    if (document.getElementById('Wage').value == '') {
        document.getElementById('WageMessage').innerText = 'اجباری می باشد';
        return false;
    } else {
        if (document.getElementById('WageMessage') != null)
            document.getElementById('WageMessage').innerText = '';
    }
    if (document.getElementById('StonePrice').value == '') {
        document.getElementById('StonePriceMessage').innerText = 'اجباری می باشد';
        return false;
    } else {
        if (document.getElementById('StonePriceMessage') != null)
            document.getElementById('StonePriceMessage').innerText = '';
    }
    if (document.getElementById('GalleryProfit').value == '') {
        document.getElementById('GalleryProfitMessage').innerText = 'اجباری می باشد';
        return false;
    } else {
        if (document.getElementById('GalleryProfitMessage') != null)
            document.getElementById('GalleryProfitMessage').innerText = '';
    }
    if (document.getElementById('Tax').value == '') {
        document.getElementById('TaxMessage').innerText = 'اجباری می باشد';
        return false;
    } else {
        if (document.getElementById('TaxMessage') != null)
            document.getElementById('TaxMessage').innerText = '';
    }
    return true;
}


calculateGoldPrice = (form) => {
    try {
        if (event != undefined)
            event.preventDefault();
        if (validatecalculateGoldPriceForm()) {
            $.ajax({
                type: "POST",
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response != undefined) {
                        if (response.IsSuccess) {
                            document.getElementById('pureGoldPrice').innerHTML = response.Data.PureGoldPrice;
                            document.getElementById('finalInvoicePrice').innerHTML = response.Data.InvoiceTotalPrice;
                        }
                    }
                },
                error: function (err) {
                    ShowAlertToast(2, '', err);
                }
            });
            validatecalculateGoldPriceForm()
        }
    } catch (e) {
        ShowAlertToast(2, '', e);
    }
    return false;
}

function setGodPriceTimer() {

    setInterval(() => {
        let date = new Date()
        let min = date.getMinutes();
        let hour = date.getHours();
        let second = date.getSeconds();
        try {
            document.getElementById('timer-hour').innerHTML = hour < 10 ? `0${hour}` : hour;
            document.getElementById('timer-min').innerHTML = min < 10 ? `0${min}` : min;
            document.getElementById('timer-second').innerHTML = second < 10 ? `0${second}` : second;
        } catch (e) {

        }
    }, 1000);
}

function monyFormat(input) {
    var separator = ",";
    var int = input.value.replace(new RegExp(separator, "g"), "");
    var regexp = new RegExp("\\B(\\d{3})(" + separator + "|$)");
    do {
        int = int.replace(regexp, separator + "$1");
    }
    while (int.search(regexp) >= 0)

    input.value = int;
}

function floatNumber(evt, elementId) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    var numberInput = document.getElementById(elementId).value;
    if (charCode !== 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    var dots = numberInput.split('.').length;
    if (dots > 1 && charCode === 46) {
        return false;
    }
    return true;
}

function renderFavoriteSection(productId, isFavorite) {
    if (isFavorite) {
        return ` <i id='favoritState' onclick="unFavorite('${productId}','favoritState')" class="fa fa-bookmark ms-3"></i>`
    } else {
        return ` <i id='favoritState' onclick="addToFavorite('${productId}','favoritState')" class="fa-light fa-bookmark ms-3"></i>`
    }
}

addToFavorite = (productId, elementId) => {
    try {
        var form = new FormData();
        form.append("productId", productId);
        $.ajax({
            type: "POST",
            url: '/User/AddToFavorites',
            data: form,
            contentType: false,
            processData: false,
            statusCode: {
                403: function (xhr) {
                    ShowToastAlert(2, 'برای افزودن این محصول به علاقه مندی ها، ابتدا وارد سایت شوید');
                }
            },
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        document.getElementById('favoriteSection').innerHTML = renderFavoriteSection(productId, true);
                    }
                }
                ShowToastAlert(response.Type, response.Message)
            },
            error: function (err) {
               // ShowToastAlert(2, err);
            }
        });
    } catch (e) {
        ShowToastAlert(2, e);
    }
    return false;
}

unFavorite = (productId, isUpdaetList = false) => {
    try {
        var form = new FormData();
        form.append("productId", productId);
        $.ajax({
            type: "POST",
            url: '/User/UnFavorite',
            data: form,
            contentType: false,
            processData: false,
            statusCode: {
                403: function (xhr) {
                    ShowToastAlert(2, 'برای حذف این محصول از علاقه مندی ها، ابتدا وارد سایت شوید');
                }
            },
            success: function (response) {
                debugger
                if (response != undefined) {
                    if (response.IsSuccess) {
                        document.getElementById('favoriteSection').innerHTML = renderFavoriteSection(productId, false);
                        if (isUpdaetList) {
                            const element = document.getElementById(`productItem${productId}`);
                            element.remove();
                            $('#productInfo').modal('hide');
                        }
                    }
                }
                ShowToastAlert(response.Type, response.Message)
            },
            error: function (err) {
                //  ShowAlertToast(2, err);
            },
        });
    } catch (e) {
        ShowAlertToast(2, e);
    }
    return false;
}

function changeNatText() {
    var e = document.getElementById("NationalityType");
    var value = e.value;
    if (value == '3') {
        document.getElementById('NationalCode').setAttribute('placeholder', 'کد شناسایی');
    } else if (value == '1') {
        document.getElementById('NationalCode').setAttribute('placeholder', 'کد ملی');
    }
}

function submitContactUsForm(form, confirmMessage) {
    try {
        if (event != undefined)
            event.preventDefault();
        if (confirm(confirmMessage) == true) {
            $.ajax({
                type: "POST",
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response != undefined) {
                        if (response.IsSuccess) {
                            form.reset();
                        }
                        ShowToastAlert(response.Type, response.Message)
                        refreshImage('captcha');
                    } else {
                        ShowToastAlert(1, errorInProccessDataInServer);
                    }
                },
                error: function (err) {
                    ShowToastAlert(1, errorInProccessDataInServer);
                }
            });
        }
    } catch (e) {
        ShowToastAlert(1, errorInConnectToServer);
    }
    return false;
}

//$(document).ready(function () {
//    $(function () {
//        $('#blogContentInfo').modal({
//            show: false
//        }).on('hidden.bs.modal', function () {
//            //var myVideo = document.querySelector("#blogContentInfo ");
//            document.getElementById("imageOrVideo").innerHTML = '';
//            //let vid = document.getElementById("postVideo");
//            //vid.pause();
//        });
//    });
//});



//function playVid() {
//    vid.play();
//}

//function pauseVid() {
//    vid.pause();
//}




//function loadFAQ(category, el) {
//    var categoryItems = document.getElementsByName('FAQCategory');
//    if (categoryItems.length > 0) {
//        for (var i = 0; i < categoryItems.length; i++) {
//            categoryItems[i].classList.remove('active')
//        }
//    }
//    el.classList.add('active');
//    $.ajax({
//        type: "GET",
//        url: `/GetFAQByCategory/${category}`,
//        success: function (response) {
//            if (response != undefined) {
//                if (response.IsSuccess) {
//                    document.getElementById('accordion').innerHTML = response.Data;
//                }
//            } else {
//                ShowToastAlert(1, errorInProccessDataInServer);
//            }
//        },
//        error: function (err) {
//            ShowToastAlert(1, errorInProccessDataInServer);
//        }
//    });
//    try {
        
//    } catch (e) {
//        ShowToastAlert(1, errorInConnectToServer);
//    }
//    return false;
//}








//$("#accordion > li > div").click(function () {
//    $('.active').not(this).removeClass('active').next().hide(300);

//    $(this).toggleClass('active');
//    if (false == $(this).next().is(':visible')) {
//        $('#accordion > ul').slideUp(300);
//    }
//    $(this).next().slideToggle(300);
//});





//var animationIsOff = $.fx.off;
//$.fx.off = true;
//$('#accordion > li > div:eq(0)').click()
//$.fx.off = animationIsOff;


$("#my-accordion > div > div.accordion-title").click(function () {
    //$(this).removeClass('border-bottom-0');
   
    $('div.accordion-title').not(this).removeClass('active', 'border-bottom-0').addClass('border-bottom-0').next().hide(300);
   // $('div.accordion-title>span>i').not(this).removeClass('fa-chevron-up').addClass('fa-chevron-down').next().hide(300);

   // $('div.accordion-title>span>i').not(this).removeClass('fa-chevron-down');
    //$('div.accordion-title>span>i').addClass('fa-chevron-up');
    //$('div.accordion-title').addClass('border-bottom-0');


   // this.target.querySelector('div.accordion-title>span>i').removeClass('fa-chevron-down').addClass('fa-chevron-up');
    //$('div.accordion-title>span>i').addClass('fa-chevron-up').removeClass('fa-chevron-down');

   /* $('.active').removeClass('active').next().hide(300);*/

   // $('div.accordion-title>span>i').removeClass('fa-chevron-down').removeClass('fa-chevron-up');
    const iTag = this.querySelector('i');
    $(this).toggleClass('active');
    if (false == $(this).next().is(':visible')) {
        //up
        //$('#my-accordion> div > div.card-header').slideUp(300);
        $(this).removeClass('border-bottom-0');
       // $(this).children('span>i').removeClass('fa-chevron-up').addClass('fa-chevron-down');
       
        iTag.classList.add('fa-chevron-up');
        iTag.classList.remove('fa-chevron-down');
        
    } else {
        //down
        $(this).addClass('border-bottom-0');
        iTag.classList.remove('fa-chevron-up');
        iTag.classList.add('fa-chevron-down');
       // $(this).children('span>i').addClass('fa-chevron-up').removeClass('fa-chevron-down');
    }
    $(this).next().slideToggle(300);
});
var animationIsOff = $.fx.off;
$.fx.off = true;
$('#my-accordion > div > div> div.accordion-content:eq(0)').click()
$.fx.off = animationIsOff;

//$('div.accordion-title').removeClass('active');