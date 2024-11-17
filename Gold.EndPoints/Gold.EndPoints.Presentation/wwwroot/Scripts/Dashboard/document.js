$(document).ready(function () {
    $('select').select2();
    $('#DocumentDate').mask('9999/99/99')
});
//Document object
let newDocument = {

    CustomerId: 0,
    GalleryId: '',
    GalleryName: '',
    SellerId: '',
    SellerName: '',
    PrepaymentAmount: '',
    RemainAmount: '',
    InstallmentCount: '',
    InstallmentAmount: '',
    AdminDescription: '',
    DelayAmount: '',
    FactorAmount: 0,
    CityName: '',
    ProvinceName: '',
    IsNewCstomer: false,

    DocumentNo: '',
    DocumentDate: '',
    Nationality: '',
    NationalCode: '',

    FullName: '',
    Gender: '',
    FatherName: '',
    Mobile: '',
    EssentialTel: '',
    EssentialTelRatio: '',
    BirthDate: '',
    JobTitle: '',
    PostalCode: '',
    CityId: '',
    Address: '',
    CardNumberOwner: '',
    CardNumber: ''
};
let InstallmentList = [];
let GuaranteeList = [];
var isValidStep3 = true;
var guranteeId = 1;
let Installment = {
    Row: 0,
    PaymentDate: '',
    DelayDays: '',
    PaymentAmount: '',
    Description: ''
};

let DocumentList = [];

function processStep1(docDateConfirmMessage) {
    var documentNumber = document.getElementById('DocumentNumber').value;
    var documentDate = document.getElementById('DocumentDate').value;
    var nationalCode = document.getElementById('NationalCode').value;
    var nationality = document.getElementById('Nationality').value;
    //validate processStep1 document create date
    $.ajax({
        type: "Get",
        url: '/Dashboard/Document/ValidateDocDate',
        data: { documentDate: documentDate },
        success: function (response) {
           debugger
            if (response != undefined) {
                if (response.IsSuccess) {
                    newDocument.DocumentDate = documentDate;
                    if (response.Data) {
                        //docDate>now
                        if (confirm(docDateConfirmMessage)) {
                            //validate step1 data
                            $.ajax({
                                type: "Get",
                                url: '/Dashboard/Document/ValidateStepOne',
                                data: { docNumber: documentNumber, nationalCode: nationalCode, documentDate: documentDate, nationality: nationality },
                                success: function (response) {
                                    if (response != undefined) {
                                        if (response.IsSuccess) {
                                            newDocument.DocumentNo = documentNumber;
                                            newDocument.DocumentDate = documentDate;
                                            newDocument.NationalCode = nationalCode;
                                            newDocument.Nationality = nationality;
                                            if (response.Data) {
                                                //نمایش اطلاعات مشتری
                                                //show step 2-1
                                                newDocument.IsNewCstomer = false;
                                                fillStep2_1Data(nationalCode);
                                                showStep2_1();
                                            } else {
                                                //مشتری جدید
                                                //show step 2-2
                                                newDocument.IsNewCstomer = true;
                                                showStep2_2();
                                            }
                                        } else {
                                            ShowAlertToast(response.Type, response.Title, response.Message);
                                        }
                                    } else {
                                        ShowAlertToast(1, errorInProccessDataInServer, response);
                                        isExist = false;
                                    }
                                },
                                error: function (err) {
                                    ShowAlertToast(1, errorInConnectToServer, err);
                                    isExist = false;
                                }
                            });
                        } 
                    } else {
                          //docDate<now
                        //validate step1 data
                        $.ajax({
                            type: "Get",
                            url: '/Dashboard/Document/ValidateStepOne',
                            data: { docNumber: documentNumber, nationalCode: nationalCode, documentDate: documentDate, nationality: nationality },
                            success: function (response) {
                                if (response != undefined) {
                                    if (response.IsSuccess) {
                                        newDocument.DocumentNo = documentNumber;
                                        newDocument.DocumentDate = documentDate;
                                        newDocument.NationalCode = nationalCode;
                                        newDocument.Nationality = nationality;
                                        if (response.Data) {
                                            //نمایش اطلاعات مشتری
                                            //show step 2-1
                                            newDocument.IsNewCstomer = false;
                                            fillStep2_1Data(nationalCode);
                                            showStep2_1();
                                        } else {
                                            //مشتری جدید
                                            //show step 2-2
                                            newDocument.IsNewCstomer = true;
                                            showStep2_2();
                                        }
                                    } else {
                                        ShowAlertToast(response.Type, response.Title, response.Message);
                                    }
                                } else {
                                    ShowAlertToast(1, errorInProccessDataInServer, response);
                                    isExist = false;
                                }
                            },
                            error: function (err) {
                                ShowAlertToast(1, errorInConnectToServer, err);
                                isExist = false;
                            }
                        });
                    }
                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            } else {
                ShowAlertToast(1, errorInProccessDataInServer, response);
                isExist = false;
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInConnectToServer, err);
            isExist = false;
        }
    });

   
}


function processStep2_1() {
    //پر کردن فرم اطلاعات سند
    //fill step3 parameters
    //get updated data from derver
    $.ajax({
        type: "Get",
        url: `/Dashboard/Document/GetCustomerInfo?nationalCode=${newDocument.NationalCode}`,
        success: function (response) {
            if (response != undefined) {
                if (response.IsSuccess) {
                    //پر کردن اطلاعات مشتری در اطلاعات سند
                    newDocument.FullName = response.Data.FullName;
                    newDocument.NationalCode = response.Data.NationalCode;
                    newDocument.Mobile = response.Data.Mobile;
                    newDocument.EssentialTel = response.Data.EssentialTel;

                    document.getElementById('lable_fullname').innerHTML = newDocument.FullName;
                    document.getElementById('lable_nationalCode').innerHTML = newDocument.NationalCode;
                    document.getElementById('lable_mobile').innerHTML = newDocument.Mobile;
                    document.getElementById('lable_essentialTel').innerHTML = newDocument.EssentialTel;
                    document.getElementById('lable_documentNumebr').innerHTML = newDocument.DocumentNo;
                    document.getElementById('lable_documentDate').innerHTML = newDocument.DocumentDate;

                    showStep3();
                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            } else {
                ShowAlertToast(1, errorInProccessDataInServer, response);
                isExist = false;
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInConnectToServer, err);
            isExist = false;
        }
    });
}


loadDataAndReProcessStep1 = (url, targetId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        $.ajax({
            type: "Get",
            url: url,
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById(targetId).innerHTML = response.Data;
                    processStep1();
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

submitFormCustomerSummaryInfoAndReProcessStep1 = (form, targetId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        var id = document.getElementById('CustomerId').value;
        var mobile = document.getElementById('CustomerMobile').value;

        $.ajax({
            type: "Get",
            url: "/Dashboard/Customer/IsDuplicateCustomerMobile",
            data: { id: id, mobile: mobile },
            success: function (response) {
                if (response != undefined) {
                    if (response) {
                        if (confirm('شماره تلفن همراه وارد شده برای مشتری دیگری ثبت شده است، آیا مایل به ثبت اطلاعات می باشید؟') == true) {
                            $.ajax({
                                type: "POST",
                                url: form.action,
                                data: new FormData(form),
                                contentType: false,
                                processData: false,
                                success: function (response) {
                                    if (response != undefined) {
                                        if (response.IsSuccess) {
                                            document.getElementById(targetId).innerHTML = response.Data;
                                            processStep1();
                                        }
                                        ShowAlertToast(response.Type, response.Title, response.Message);
                                    }
                                },
                                error: function (err) {
                                    ShowAlertToast(1, errorInProccessDataInServer, err);
                                }
                            });
                        }
                    } else {
                        $.ajax({
                            type: "POST",
                            url: form.action,
                            data: new FormData(form),
                            contentType: false,
                            processData: false,
                            success: function (response) {
                                if (response != undefined) {
                                    if (response.IsSuccess) {
                                        document.getElementById(targetId).innerHTML = response.Data;
                                        processStep1();
                                    }
                                    ShowAlertToast(response.Type, response.Title, response.Message);
                                }
                            },
                            error: function (err) {
                                ShowAlertToast(1, errorInProccessDataInServer, err);
                            }
                        });
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
        ShowAlertToast(2, errorInConnectToServer, e);
    }
    return false;
}

submitFormAndShowModal3 = (form, targetModalId, targetId) => {
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
                        document.getElementById(targetId).innerHTML = response.Data;
                        processStep1();
                    }

                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            },
            error: function (err) {
                ShowAlertToast(2, errorInProccessDataInServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(2, errorInConnectToServer, e);
    }
    return false;
}

function editCustomerInfo(targetModalId) {
    try {
        if (event != undefined)
            event.preventDefault();
        var modalId = '#' + targetModalId;
        var modalBodyId = targetModalId + '_body';
        $.ajax({
            type: "Get",
            url: '/Dashboard/Document/EditCustomerSummaryInfo',
            data: { nationalCode: newDocument.NationalCode },
            success: function (response) {
                if (response.IsSuccess) {
                    document.getElementById(modalBodyId).innerHTML = response.Data;
                    $(modalId).modal('show');
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

function processStep2_2() {

    newDocument.FullName = document.getElementById("FullName").value;
    newDocument.Gender = document.getElementById("Gender").value;
    newDocument.FatherName = document.getElementById("FatherName").value;

    newDocument.Mobile = document.getElementById("Mobile").value;
    newDocument.EssentialTel = document.getElementById("EssentialTel").value;
    newDocument.EssentialTelRatio = document.getElementById("EssentialTelRatio").value;

    newDocument.BirthDate = document.getElementById("BirthDate").value;
    newDocument.JobTitle = document.getElementById("JobTitle").value;
    newDocument.PostalCode = document.getElementById("PostalCode").value;

    newDocument.CityId = document.getElementById("CityId").value;
    newDocument.Address = document.getElementById("Address").value;

    newDocument.CardNumber = document.getElementById("CardNumber").value;
    newDocument.CardNumberOwner = document.getElementById("CardNumberOwner").value;

    //validate step 2-2 data
    $.ajax({
        type: "Get",
        url: "/Dashboard/Customer/IsDuplicateCustomerMobile",
        data: { id: 0, mobile: newDocument.Mobile },
        success: function (response) {
            if (response != undefined) {
                if (response) {
                    if (confirm('شماره تلفن همراه وارد شده برای مشتری دیگری ثبت شده است، آیا مایل به ثبت اطلاعات می باشید؟') == true) {
                        validateSecondStepData();
                    }
                } else {
                    validateSecondStepData();
                }
            } else {
                ShowAlertToast(1, errorInProccessDataInServer, data);
                return false;
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInProccessDataInServer, err);
            return false;
        }
    });

}

function validateSecondStepData() {
    var formData = new FormData();
    formData.append('FullName', newDocument.FullName);
    formData.append('Gender', newDocument.Gender);
    formData.append('FatherName', newDocument.FatherName);
    formData.append('Mobile', newDocument.Mobile);
    formData.append('EssentialTel', newDocument.EssentialTel);
    formData.append('EssentialTelRatio', newDocument.EssentialTelRatio);
    formData.append('BirthDate', newDocument.BirthDate);
    formData.append('JobTitle', newDocument.JobTitle);
    formData.append('PostalCode', newDocument.PostalCode);
    formData.append('CityId', newDocument.CityId);
    formData.append('Address', newDocument.Address);
    formData.append('CardNumber', newDocument.CardNumber);
    formData.append('CardNumberOwner', newDocument.CardNumberOwner);
    $.ajax({
        type: "POST",
        url: `/Dashboard/Document/ValidateSecondStep`,
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response != undefined) {
                if (response.IsSuccess) {
                    document.getElementById('lable_fullname').innerHTML = newDocument.FullName;
                    document.getElementById('lable_mobile').innerHTML = newDocument.Mobile;
                    document.getElementById('lable_essentialTel').innerHTML = newDocument.EssentialTel;
                    document.getElementById('lable_nationalCode').innerHTML = newDocument.NationalCode;
                    document.getElementById('lable_documentNumebr').innerHTML = newDocument.DocumentNo;
                    document.getElementById('lable_documentDate').innerHTML = newDocument.DocumentDate;
                    showStep3();
                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            } else {
                ShowAlertToast(1, errorInProccessDataInServer, response);
                return false;
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInConnectToServer, err);
            return false;
        }
    });
}

function processStep3() {
    //process step3 data
    var installmentAmount = document.getElementById('InstallmentAmount').value;
    var installmentCount = document.getElementById('InstallmentCount').value;
    var remainAmount = document.getElementById('RemainAmount').value;
    var prepaymentAmount = document.getElementById('PrepaymentAmount').value;
    var adminDescription = document.getElementById('AdminDescription').value;

    $.ajax({
        type: "Get",
        url: '/Dashboard/Document/GetInstallmentsDate',
        data: { installmentCount: installmentCount, installmentDate: newDocument.DocumentDate },
        success: function (response) {
            if (response != undefined) {
                if (response.IsSuccess) {
                    //step 3 data is valid
                    newDocument.InstallmentAmount = installmentAmount;
                    newDocument.InstallmentCount = installmentCount;
                    newDocument.RemainAmount = remainAmount;
                    newDocument.AdminDescription = adminDescription;
                    newDocument.PrepaymentAmount = prepaymentAmount;
                    newDocument.SellerName = $("#Seller option:selected").text();
                    newDocument.GalleryName = $("#Gallery option:selected").text();
                    newDocument.GalleryId = document.getElementById("Gallery").value;
                    newDocument.SellerId = document.getElementById("Seller").value;
                    if (newDocument.InstallmentAmount == '' ||
                        newDocument.InstallmentCount == '' ||
                        newDocument.InstallmentCount < 1 ||
                        newDocument.RemainAmount == '' ||
                        newDocument.PrepaymentAmount == '' ||
                        newDocument.GalleryId == '' ||
                        newDocument.GalleryId == 'انتخاب کنید' ||
                        newDocument.SellerId == '' ||
                        newDocument.SellerId == 'انتخاب کنید' ||
                        GuaranteeList.length < 1) {
                        if (GuaranteeList.length < 1) {
                            ShowAlertToast(1, 'هشدار', 'حداقل یک ضمانت برای سند باید ثبت شود');
                        }
                        isValidStep3 = false;
                    } else {
                        isValidStep3 = true;
                    }

                    InstallmentList = [];
                    for (var i = 0; i < response.Data.length; i++) {
                        InstallmentList.push({
                            Row: response.Data[i].Row,
                            DelayDays: 0,
                            Description: '',
                            InstalmentDate: response.Data[i].PersianDate,
                            PaymentAmount: newDocument.InstallmentAmount
                        });
                    }
                    if (isValidStep3) {
                        document.getElementById('installment_list').innerHTML = '';
                        for (var i = 0; i < InstallmentList.length; i++) {
                            var element = `<tr>
                                    <td>${InstallmentList[i].Row}</td>
                                    <td>${InstallmentList[i].InstalmentDate}</td>
                                    <td></td>
                                    <td></td>
                                    <td>${commaSeparateNumber(newDocument.InstallmentAmount)}</td>
                                    <td><input value='${InstallmentList[i].Row}' class='checkbox-disable' type="checkbox"></input></td>
                                    <td></td>
                                </tr>`;
                            document.getElementById('installment_list').innerHTML += element;
                        }
                        document.getElementById('step4_nationalcode').innerHTML = newDocument.NationalCode;
                        document.getElementById('step4_docDate').innerHTML = newDocument.DocumentDate;
                        document.getElementById('step4_fullname').innerHTML = newDocument.FullName;
                        document.getElementById('step4_mobile').innerHTML = newDocument.Mobile;
                        document.getElementById('step4_essentiatel').innerHTML = newDocument.EssentialTel;
                        document.getElementById('step4_docNumber').innerHTML = newDocument.DocumentNo;
                        document.getElementById('step4_factorAmount').innerHTML = commaSeparateNumber(newDocument.FactorAmount);
                        document.getElementById('step4_prePaymentAmount').innerHTML = commaSeparateNumber(newDocument.PrepaymentAmount);
                        document.getElementById('step4_remainAmount').innerHTML = commaSeparateNumber(newDocument.RemainAmount);
                        document.getElementById('step4_installmentAmount').innerHTML = commaSeparateNumber(newDocument.InstallmentAmount);
                        document.getElementById('step4_installmentCount').innerHTML = newDocument.InstallmentCount;
                        document.getElementById('step4_adminDesription').innerHTML = newDocument.AdminDescription;
                        document.getElementById('step4_gallery').innerHTML = newDocument.GalleryName;
                        document.getElementById('step4_seller').innerHTML = newDocument.SellerName;

                        showStep4();
                    } else {
                        ShowAlertToast(1, 'هشدار', 'لطفا اطلاعات سند را به درستی وارد کنید.');
                    }

                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            } else {
                ShowAlertToast(1, 'خطا در محاسبه اقساط.', response);
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInProccessDataInServer, err);
            isExist = false;
        }
    });
}

function isDuplicateCustomerMobile(mobile, id) {
    $.ajax({
        type: "Get",
        url: "/Dashboard/Customer/IsDuplicateCustomerMobile",
        data: { id: id, mobile: mobile },
        success: function (response) {
            if (response != undefined) {
                if (response) {
                    if (confirm('شماره تلفن همراه وارد شده برای مشتری دیگری ثبت شده است، آیا مایل به ثبت اطلاعات می باشید؟') == true) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
            } else {
                ShowAlertToast(1, errorInProccessDataInServer, data);
                return false;
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInProccessDataInServer, err);
            return false;
        }
    });
}

function processStep4() {
    //send document data to server
    try {
        if (event != undefined)
            event.preventDefault();
        document.getElementById('btnSubmit').disabled = true;
        $.ajax({
            type: "Get",
            url: "/Dashboard/Customer/IsDuplicateCustomerMobile",
            data: { id: newDocument.CustomerId, mobile: newDocument.Mobile },
            success: function (response) {
                if (response != undefined) {
                    if (response) {
                        if (confirm('شماره تلفن همراه وارد شده برای مشتری دیگری ثبت شده است، آیا مایل به ثبت اطلاعات می باشید؟') == true) {
                            submitDocumentDataForCreate();
                        } else {
                            document.getElementById('btnSubmit').disabled = false;
                        }
                    } else {
                        submitDocumentDataForCreate();
                    }
                } else {
                    document.getElementById('btnSubmit').disabled = false;
                    ShowAlertToast(1, errorInProccessDataInServer, data);
                    return false;
                }
            },
            error: function (err) {
                document.getElementById('btnSubmit').disabled = false;
                ShowAlertToast(1, errorInProccessDataInServer, err);
                return false;
            }
        });

    } catch (e) {
        document.getElementById('btnSubmit').disabled = false;
        ShowAlertToast(2, errorInConnectToServer, e);
    }
    return false;
}

function submitDocumentDataForCreate() {
    var formData = new FormData();
    formData.append('DocumentNo', newDocument.DocumentNo);
    formData.append('DocumentDate', newDocument.DocumentDate);
    formData.append('NationalCode', newDocument.NationalCode);
    formData.append('Nationality', newDocument.Nationality);

    formData.append('FullName', newDocument.FullName);
    formData.append('Gender', newDocument.Gender);
    formData.append('FatherName', newDocument.FatherName);
    formData.append('Mobile', newDocument.Mobile);
    formData.append('EssentialTel', newDocument.EssentialTel);
    formData.append('EssentialTelRatio', newDocument.EssentialTelRatio);
    formData.append('BirthDate', newDocument.BirthDate);
    formData.append('JobTitle', newDocument.JobTitle);
    formData.append('PostalCode', newDocument.PostalCode);
    formData.append('CityId', newDocument.CityId);
    formData.append('Address', newDocument.Address);
    formData.append('CardNumber', newDocument.CardNumber);
    formData.append('CardNumberOwner', newDocument.CardNumberOwner);

    formData.append('GalleryId', newDocument.GalleryId);
    formData.append('SellerId', newDocument.SellerId);
    formData.append('PrepaymentAmount', newDocument.PrepaymentAmount);
    formData.append('RemainAmount', newDocument.RemainAmount);
    formData.append('InstallmentCount', newDocument.InstallmentCount);
    formData.append('InstallmentAmount', newDocument.InstallmentAmount);
    formData.append('AdminDescription', newDocument.AdminDescription);
    formData.append('IsNewCstomer', newDocument.IsNewCstomer);
    for (var i = 0; i < GuaranteeList.length; i++) {
        formData.append(`Collaterals[${i}].ImageFile`, GuaranteeList[i].file)
        formData.append(`Collaterals[${i}].Description`, GuaranteeList[i].description)
        formData.append(`Collaterals[${i}].CollateralTypeId`, GuaranteeList[i].typeId)
    }
    $.ajax({
        type: "POST",
        url: '/Dashboard/Document/CreateDocument',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response != undefined) {
                ShowAlertToast(response.Type, response.Title, response.Message);
                if (response.IsSuccess) {
                    setTimeout(() => { window.location.reload(); }, 1000);
                    document.getElementById('frmCreateDocument').reset();
                } else {
                    document.getElementById('btnSubmit').disabled = false;
                }
            }
        },
        error: function (err) {
            document.getElementById('btnSubmit').disabled = false;
            ShowAlertToast(2, errorInProccessDataInServer, err);
        }
    });
}


function backToStep2() {
    if (newDocument.IsNewCstomer) {
        //show step 2-2
        showStep2_2();
    } else {
        //show step 2-1
        showStep2_1();
    }
}

function sort_by_id() {
    return function (elem1, elem2) {
        if (elem1.id < elem2.id) {
            return -1;
        } else if (elem1.id > elem2.id) {
            return 1;
        } else {
            return 0;
        }
    };
}


//gurantee
function loadGuaranteeList() {
    document.getElementById('step4_Installments').innerHTML = '';
    document.getElementById('step3_Installments').innerHTML = '';
    document.getElementById('guranteeListtableBoody').innerHTML = '';
    for (var i = 0; i < GuaranteeList.length; i++) {
        var imgTag = "";
        if (GuaranteeList[i].imageFile.length > 0) {
            imgTag = `<img src='${GuaranteeList[i].imageFile}' />`;
        }
        let item = `<tr>
                       <td class='td-5'>${i + 1}</td >
                       <td class='td-10'>${GuaranteeList[i].type}</td>
                       <td class='td-40'>${GuaranteeList[i].description}</td>
                       <td class='td-25'>${imgTag}</td>
                       <td class='td-20'>
                           <div class="text-align" >
                               <button type="button" class="btn btn-primary" onclick='getForEditGuarantee("${GuaranteeList[i].id}")'><i class="fa fa-edit"></i></button>
                               <button type="button" class="btn btn-danger" onclick='deleteGuarantee("${GuaranteeList[i].id}")'><i class="fa fa-trash"></i></button>
                           </div> 
                       </td>
                    </tr >`;
        document.getElementById('guranteeListtableBoody').innerHTML += item;
        let collactrealSummary = '';
        if (GuaranteeList[i].description.length > 0) {
            collactrealSummary = `${GuaranteeList[i].type} - ${GuaranteeList[i].description}<br>`
        } else {
            collactrealSummary = `${GuaranteeList[i].type}<br>`
        }
        document.getElementById('step4_Installments').innerHTML += collactrealSummary;
        document.getElementById('step3_Installments').innerHTML += collactrealSummary;
    }
}

function deleteGuarantee(id) {
    try {
        for (var i = 0; i < GuaranteeList.length; i++) {
            if (GuaranteeList[i].id == id) {
                if (confirm("آیا از انجام عملیات مطمئن هستید؟") == true) {
                    GuaranteeList.splice(i, 1);
                    loadGuaranteeList();
                    ShowAlertToast(0, 'عملیات موفق', 'ضمانت با موفقیت حذف شد');
                } else {
                    return false;
                }
            }
        }

    } catch (e) {
        ShowAlertToast(1, 'خطا', 'مشکلی در حذف ضمانت پیش آمد');
    }
}

function getForEditGuarantee(id) {
    var gurantee = GuaranteeList.find(x => x.id == id);
    $("#guranteeType").val(gurantee.typeId).change();
    document.getElementById('gurranteeId').value = gurantee.id;
    document.getElementById('guranteeDescription').value = gurantee.description;
    document.getElementById('guranteeImageFile').src = `${gurantee.imageFile}`;
}

$(document).ready(function () {
    const input = document.getElementById('guranteeFile');
    try {
        input.addEventListener('change', (event) => {
            const image = event.target.files[0];
            const reader = new FileReader();
            reader.readAsDataURL(image);
            reader.addEventListener('load', () => {
                localStorage.setItem('guranteeFile', reader.result);
            });
        });
    } catch (e) {
        /*  ShowAlertToast(1, 'خطا', 'مشکلی در افزودن ضمانت پیش آمد');*/
    }
});


function addGuarantee() {
    try {
        var gurranteeId = document.getElementById('gurranteeId').value;
        var type = $("#guranteeType option:selected").text();
        var typeId = document.getElementById('guranteeType').value;
        var description = document.getElementById('guranteeDescription').value;
        var guranteeFile = document.getElementById('guranteeFile').value
        var imageFile = '';
        var file;

        if (typeId == "") {
            ShowAlertToast(2, 'هشدار', 'نوع ضمانت را انتخاب کنید');
        } else {
            if (gurranteeId != '') {
                var gurantee = GuaranteeList.find(x => x.id == gurranteeId);
                //edit
                gurantee.typeId = typeId;
                gurantee.type = type;
                gurantee.description = description;
                if (guranteeFile != '') {
                    gurantee.file = document.getElementById('guranteeFile').files[0];
                    gurantee.imageFile = localStorage.getItem('guranteeFile');
                }

            } else {
                //add
                if (guranteeFile != '') {
                    imageFile = localStorage.getItem('guranteeFile');
                    file = document.getElementById('guranteeFile').files[0]
                }
                GuaranteeList.push({
                    id: guranteeId,
                    typeId: typeId,
                    type: type,
                    description: description,
                    imageFile: imageFile,
                    file: file
                });
                guranteeId += 1;
            }
            $("#guranteeType").val('').change();
            document.getElementById('guranteeDescription').value = '';
            document.getElementById('guranteeFile').value = '';
            document.getElementById('gurranteeId').value = '';
            document.getElementById('guranteeImageFile').src = '';
            localStorage.setItem('guranteeFile', '');

            loadGuaranteeList();
            ShowAlertToast(0, 'عملیات موفق', 'ضمانت با موفقیت ثبت شد');
        }
    } catch (e) {
        ShowAlertToast(1, 'خطا', 'مشکلی در افزودن ضمانت پیش آمد');
    }

}
//end gurantee

function b64toBlob(b64Data, contentType = '', sliceSize = 512) {
    const byteCharacters = atob(b64Data);
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        const slice = byteCharacters.slice(offset, offset + sliceSize);

        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }

    const blob = new Blob(byteArrays, { type: contentType });
    return blob;
}

function getBase64(input) {
    return input.replace(/^data:image\/(png|jpg);base64,/, "");
}

function getBase64Image(img) {
    var canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;

    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0);

    var dataURL = canvas.toDataURL("image/png");

    return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
}



function fillStep2_1Data(nationalCode) {
    $.ajax({
        type: "Get",
        url: `/Dashboard/Document/GetCustomerInfo?nationalCode=${nationalCode}`,
        success: function (response) {
            if (response != undefined) {
                if (response.IsSuccess) {
                    //fill data
                    if (response.Data != undefined) {
                        newDocument.CustomerId = response.Data.CustomerId;
                        newDocument.FullName = response.Data.FullName;
                        newDocument.NationalCode = response.Data.NationalCode;
                        newDocument.FatherName = response.Data.FatherName;
                        newDocument.NationalCode = response.Data.NationalCode;
                        newDocument.Mobile = response.Data.Mobile;
                        newDocument.EssentialTel = response.Data.EssentialTel;
                        newDocument.EssentialTelRatio = response.Data.EssentialTelRatio;
                        newDocument.CityName = response.Data.CityName;
                        newDocument.ProvinceName = response.Data.ProvinceName;
                        newDocument.Address = response.Data.Address;
                        //fill documents list
                        DocumentList = [];
                        for (let index = 0; index < response.Data.Documents.length; ++index) {
                            const item = response.Data.Documents[index];
                            DocumentList.push({ Id: item.Id, DocumentNo: item.DocumentNo, Status: item.Status, Color: item.RowColor })
                        }

                        document.getElementById('step2_1_fullname').innerHTML = newDocument.FullName;
                        document.getElementById('step2_1_nationalCode').innerHTML = newDocument.NationalCode;
                        document.getElementById('step2_1_fatherName').innerHTML = newDocument.FatherName;
                        document.getElementById('step2_1_mobile').innerHTML = newDocument.Mobile;
                        document.getElementById('step2_1_essentialTel').innerHTML = newDocument.EssentialTel;
                        document.getElementById('step2_1_essentialTelRatio').innerHTML = newDocument.EssentialTelRatio;
                        document.getElementById('step2_1_city').innerHTML = newDocument.CityName;
                        if (newDocument.ProvinceName != '' & newDocument.ProvinceName != null) {
                            document.getElementById('step2_1_province').innerHTML = newDocument.ProvinceName + "/";
                        }
                        document.getElementById('step2_1_address').innerHTML = newDocument.Address;
                        if (DocumentList.length > 0) {
                            document.getElementById('step2_1_div_documents').innerHTML = '';
                            for (var i = 0; i < DocumentList.length; i++) {
                                //let btnColorClass = setDocumentColorByStatus(DocumentList[i].Status)
                                var documentInfo = ` <a href='/Dashboard/Document/Detail?id=${DocumentList[i].Id}' class='btn ${DocumentList[i].Color}' target='_blank'>سند شماره ${DocumentList[i].DocumentNo}</a>`;
                                document.getElementById('step2_1_div_documents').innerHTML += documentInfo;
                            }
                        }
                    }

                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            } else {
                ShowAlertToast(1, errorInProccessDataInServer, response);
                return false;
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInConnectToServer, err);
            return false;
        }
    });
}

function setDocumentColorByStatus(statusCode) {
    if (statusCode == 2) {
        return "btn-success";
    } else if (statusCode == 1) {
        return "btn-info";
    } else if (statusCode == 3) {
        return "btn-danger";
    }
}

function InstallmentCountValidate() {
    //validate installment count
    var value = document.getElementById('InstallmentCount').value;
    if (value == undefined) {
        document.getElementById('InstallmentCountValResult').innerHTML = "تعداد اقساط را وارد نمایید"

    } else {
        if (value <= 0) {
            document.getElementById('InstallmentCountValResult').innerHTML = "تعداد اقساط باید بزرگتر از یک باشد";

        } else if (value > 250) {
            document.getElementById('InstallmentCountValResult').innerHTML = "تعداد اقساط باید کمتر از 250 باشد";

        } else {
            document.getElementById('InstallmentCountValResult').innerHTML = "";

        }
    }

}

function factorAmountCalculator() {
    //محاسبه مبلغ فاکتور
    var remainAmount = removeComma(document.getElementById('RemainAmount').value);
    var prepaymentAmount = removeComma(document.getElementById('PrepaymentAmount').value);
    if (isNaN(parseInt(remainAmount)) != true) {
        InstallmentAmountCalculator();
    }
    if (isNaN(parseInt(remainAmount)) || isNaN(parseInt(prepaymentAmount))) {

        if (document.getElementById('factorAmount').innerHTML) {

        }
        document.getElementById('factorAmount').innerHTML = '';
        return false;
    } else {

        newDocument.FactorAmount = parseInt(remainAmount) + parseInt(prepaymentAmount);
        var factorValue = `${commaSeparateNumber(newDocument.FactorAmount)} تومان`
        document.getElementById('factorAmount').innerHTML = factorValue;
    }
    return true;
}

function proccessInstallmentAmount() {
    var installmentAmountSystem = removeComma(document.getElementById('InstallmentAmountSystem').value);
    var installmentAmount = removeComma(document.getElementById('InstallmentAmount').value);

    if (installmentAmountSystem != undefined && installmentAmount != undefined) {
        if (parseInt(installmentAmount) < parseInt(installmentAmountSystem)) {
            if (confirm('مبلغ قسط کمتر از مبلغ محاسبه شده توسط سیستم است. آیا مورد تایید است؟')) {
                return true;
            } else {
                return false;
            }
        } else {
            return true;
        }
    } else {
        ShowAlertToast(1, 'خطا', 'خطا در محاسبه مبلغ قسط');
    }
}

function InstallmentAmountCalculator() {

    var installmentCount = document.getElementById('InstallmentCount').value;
    if (installmentCount == undefined) {
        if (document.getElementById('InstallmentAmountValResult') != undefined)
            document.getElementById('InstallmentAmountValResult').innerHTML = "تعداد اقساط را وارد نمایید";

    } else {
        if (installmentCount <= 0) {
            if (document.getElementById('InstallmentAmountValResult') != undefined)
                document.getElementById('InstallmentAmountValResult').innerHTML = "تعداد اقساط باید بزرگتر از یک باشد";

        } else if (installmentCount > 250) {
            if (document.getElementById('InstallmentAmountValResult') != undefined)
                document.getElementById('InstallmentAmountValResult').innerHTML = "تعداد اقساط باید کمتر از 250 باشد";

        } else {
            if (document.getElementById('InstallmentAmountValResult') != undefined)
                document.getElementById('InstallmentAmountValResult').innerHTML = "";

        }
    }
    var remainAmount = document.getElementById('RemainAmount').value.replace(",", "");

    if (remainAmount != undefined) {
        if (isNaN(parseInt(remainAmount)) == true) {
            if (document.getElementById('InstallmentAmountValResult') != undefined)
                document.getElementById('InstallmentAmountValResult').innerHTML = 'مقدار مانده را وارد نمایید';
            remainAmount = '0';

        } else {
            if (document.getElementById('InstallmentAmountValResult') != undefined)
                document.getElementById('InstallmentAmountValResult').innerHTML = '';

        }
    }
    //محاسبه مبلغ اقساط
    //var monthlyProfitPercentage = getMonthlyProfitPercentageValue();
    $.ajax({
        type: "Get",
        url: `/Dashboard/Document/GetInstallmentAmount?remainAmount=${remainAmount}&installmentCount=${installmentCount}`,
        success: function (response) {
            if (response != undefined) {
                if (response.IsSuccess) {
                    document.getElementById('InstallmentAmount').value = response.Data;
                    document.getElementById('InstallmentAmountSystem').value = response.Data;
                } else {
                    //ShowAlertToast(response.Type, response.Title, response.Message);
                    return 0;
                }
            } else {
                ShowAlertToast(1, 'Error In Load Data!', err);
                return 0;
            }
        },
        error: function (err) {
            ShowAlertToast(1, 'Error In Load Data!', err);
        }
    });

}

function InstallmentAmountCalculatorInEditDocument() {
    factorAmountCalculator();
    var installmentCount = document.getElementById('InstallmentCount').value;
    if (installmentCount == undefined) {
        ShowAlertToast(2, 'هشدار', 'تعداد اقساط را وارد نمایید');

    } else {
        if (installmentCount <= 0) {
            ShowAlertToast(2, 'هشدار', 'تعداد اقساط باید بزرگتر از یک باشد');

        } else if (installmentCount > 250) {
            ShowAlertToast(2, 'هشدار', 'تعداد اقساط باید کمتر از 250 باشد');

        }
    }
    var remainAmount = document.getElementById('RemainAmount').value.replace(",", "");

    if (remainAmount != undefined) {
        if (isNaN(parseInt(remainAmount)) == true) {
            ShowAlertToast(2, 'هشدار', 'مقدار مانده را وارد نمایید');

        }
    }
    //محاسبه مبلغ اقساط
    //var monthlyProfitPercentage = getMonthlyProfitPercentageValue();
    $.ajax({
        type: "Get",
        url: `/Dashboard/Document/GetInstallmentAmount?remainAmount=${remainAmount}&installmentCount=${installmentCount}`,
        success: function (response) {
            if (response != undefined) {
                if (response.IsSuccess) {
                    document.getElementById('InstallmentAmount').value = response.Data;

                    if (document.getElementById('InstallmentAmountSystem') != undefined) {
                        document.getElementById('InstallmentAmountSystem').value = response.Data;
                    }

                } else {
                    return 0;
                }
            } else {
                ShowAlertToast(2, 'خطا در محسابه مبلغ اقساط', err);
                return 0;
            }
        },
        error: function (err) {
            ShowAlertToast(2, 'خطا در ارتباط با سرور جهت محاسبه مبلغ اقساط', err);
        }
    });

}

function processInstallment() {
    var installmentAmount = document.getElementById('InstallmentAmount').value;
    var installmentCount = document.getElementById('InstallmentCount').value;
    var remainAmount = document.getElementById('RemainAmount').value;
    var prepaymentAmount = document.getElementById('PrepaymentAmount').value;
    var adminDescription = document.getElementById('AdminDescription').value;
    $.ajax({
        type: "Get",
        url: '/Dashboard/Document/GetInstallmentsDate',
        data: { installmentCount: installmentCount, installmentDate: newDocument.DocumentDate },
        success: function (response) {
            if (response != undefined) {
                if (response.IsSuccess) {
                    //step 3 data is valid
                    newDocument.InstallmentAmount = installmentAmount;
                    newDocument.InstallmentCount = installmentCount;
                    newDocument.RemainAmount = remainAmount;
                    newDocument.AdminDescription = adminDescription;
                    newDocument.PrepaymentAmount = prepaymentAmount;
                    newDocument.SellerName = $("#Seller option:selected").text();
                    newDocument.GalleryName = $("#Gallery option:selected").text();
                    newDocument.GalleryId = document.getElementById("Gallery").value;
                    newDocument.SellerId = document.getElementById("Seller").value;

                    for (var i = 0; i < response.Data.length; i++) {
                        InstallmentList.push({
                            Row: response.Data[i].Row,
                            DelayDays: 0,
                            Description: '',
                            InstalmentDate: response.Data[i].PersianDate,
                            PaymentAmount: newDocument.InstallmentAmount
                        });
                    }
                    document.getElementById('installment_list').innerHTML = '';
                    var checked = 'checked';
                    var unchecked = '';
                    for (var i = 0; i < InstallmentList.length; i++) {
                        var element = `<tr>
                                    <td>${InstallmentList[i].Row}</td>
                                    <td>${InstallmentList[i].InstalmentDate}</td>
                                    <td></td>
                                    <td></td>
                                    <td>${newDocument.InstallmentAmount}</td>
                                    <td><input type="checkbox" class='checkbox-disable'></input></td>
                                    <td></td>
                                </tr>`;
                        document.getElementById('installment_list').innerHTML += element;

                    }
                    document.getElementById('step4_nationalcode').innerHTML = newDocument.NationalCode;
                    document.getElementById('step4_docDate').innerHTML = newDocument.DocumentDate;
                    document.getElementById('step4_fullname').innerHTML = newDocument.FullName;
                    document.getElementById('step4_mobile').innerHTML = newDocument.Mobile;
                    document.getElementById('step4_essentiatel').innerHTML = newDocument.EssentialTel;
                    document.getElementById('step4_docNumber').innerHTML = newDocument.DocumentNo;
                    document.getElementById('step4_factorAmount').innerHTML = newDocument.FactorAmount;
                    document.getElementById('step4_prePaymentAmount').innerHTML = newDocument.PrepaymentAmount;
                    document.getElementById('step4_remainAmount').innerHTML = newDocument.RemainAmount;
                    document.getElementById('step4_installmentAmount').innerHTML = newDocument.InstallmentAmount;
                    document.getElementById('step4_gallery').innerHTML = newDocument.GalleryName;
                    document.getElementById('step4_seller').innerHTML = newDocument.SellerName;

                    showStep4();
                } else {
                    ShowAlertToast(response.Type, response.Title, response.Message);
                }
            } else {
                ShowAlertToast(1, 'خطا در محاسبه اقساط.', response);
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInProccessDataInServer, err);
            isExist = false;
        }
    });

}


function showStep1() {
    document.getElementById("step1").style.display = "";
    document.getElementById("step1_li").className = "stepy-active";

    document.getElementById("step2-1").style.display = "none";
    document.getElementById("step2-2").style.display = "none";
    document.getElementById("step2_li").className = "";

    document.getElementById("step3").style.display = "none";
    document.getElementById("step3_li").className = "";

    document.getElementById("step4").style.display = "none";
    document.getElementById("step4_li").className = "";
}
function showStep2_1() {
    document.getElementById("step1").style.display = "none";
    document.getElementById("step1_li").className = "";
    document.getElementById("step2-1").style.display = "";
    document.getElementById("step2_li").className = "stepy-active";
    document.getElementById("step2-2").style.display = "none";
    document.getElementById("step3").style.display = "none";
    document.getElementById("step3_li").className = "stepy-active";
    document.getElementById("step4").style.display = "none";
    document.getElementById("step4_li").className = "stepy-active";
}
function showStep2_2() {
    document.getElementById("step1").style.display = "none";
    document.getElementById("step1_li").className = "";
    document.getElementById("step2-1").style.display = "none";
    document.getElementById("step2-2").style.display = "";
    document.getElementById("step2_li").className = "stepy-active";

    document.getElementById("step3").style.display = "none";
    document.getElementById("step3_li").className = "";

    document.getElementById("step4").style.display = "none";
    document.getElementById("step4_li").className = "stepy-active";
}
function showStep3() {
    document.getElementById("step1").style.display = "none";
    document.getElementById("step1_li").className = "";

    document.getElementById("step2-1").style.display = "none";
    document.getElementById("step2-2").style.display = "none";
    document.getElementById("step2_li").className = "";

    document.getElementById("step3").style.display = "";
    document.getElementById("step3_li").className = "stepy-active";

    document.getElementById("step4").style.display = "none";
    document.getElementById("step4_li").className = "";

}
function showStep4() {
    if (proccessInstallmentAmount()) {
        document.getElementById("step1").style.display = "none";
        document.getElementById("step1_li").className = "";

        document.getElementById("step2-1").style.display = "none";
        document.getElementById("step2-2").style.display = "none";
        document.getElementById("step2_li").className = "";

        document.getElementById("step3").style.display = "none";
        document.getElementById("step3_li").className = "";

        document.getElementById("step4").style.display = "";
        document.getElementById("step4_li").className = "stepy-active";
    }
}



function clearInputValueById(inputId) {
    document.getElementById(inputId).value = '';
}

function deletePayment(url, confirmText, targetId) {
    try {
        if (confirm(confirmText) == true) {
            $.ajax({
                type: "Get",
                url: url,
                success: function (response) {
                    if (response != undefined) {
                        ShowAlertToast(response.Type, response.Title, response.Message);
                        if (response.IsSuccess) {
                            document.getElementById(targetId).innerHTML = response.Data;
                            setSwitcheryStyle();
                            getInstallments(documentId);
                            getInstllmentInfo(documentId);
                        }
                    }
                },
                error: function (err) {
                    ShowAlertToast(1, errorInProccessDataInServer, err);
                }
            });
        }

    } catch (e) {
        ShowAlertToast(2, errorInConnectToServer, e);
    }

}
function unPaymentInstallment(url, confirmText) {
    try {
        if (confirm(confirmText) == true) {
            $.ajax({
                type: "Get",
                url: url,
                success: function (response) {
                    if (response != undefined) {
                        ShowAlertToast(response.Type, response.Title, response.Message);
                        if (response.IsSuccess) {
                            setTimeout(() => { window.location.reload(); }, 500);
                            getInstallments(documentId)
                            getInstllmentInfo(documentId)
                        }
                    }
                },
                error: function (err) {
                    ShowAlertToast(1, errorInProccessDataInServer, err);
                }
            });
        }

    } catch (e) {
        ShowAlertToast(2, errorInConnectToServer, e);
    }

}

function editDocument(form, confirmText) {
    try {
        if (event != undefined)
            event.preventDefault();
        if (proccessInstallmentAmount()) {
            if (confirm(confirmText) == true) {
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
                                setTimeout(() => { window.location.reload(); }, 2500);
                            }
                        } else {
                            ShowAlertToast(1, errorInProccessDataInServer, err);
                        }
                    },
                    error: function (err) {
                        ShowAlertToast(2, errorInConnectToServer, e);
                    }
                });

            }
        }
    } catch (e) {
        ShowAlertToast(1, errorInConnectToServer, e);
    }
}

function delayAmountChang() {
    document.getElementById('delayAmountLable').innerHTML = document.getElementById('DelayAmount').value;
}
settleDocument = (form, targetModalId, confirmText) => {
    try {
        if (event != undefined)
            event.preventDefault();
        if (confirm(confirmText) == true) {
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
                        ShowAlertToast(response.Type, response.Title, response.Message);
                        if (response.IsSuccess) {
                            setTimeout(() => { window.location.reload(); }, 3000);
                            //getInstllmentInfo(documentId);;
                            //$(modalId).modal('hide');
                        }
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
submitFormCollateral = (form, targetModalId, collateralListId) => {
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

                        clear_form_collateral()
                        //setSelect2Style()
                        getCollateralsInfo(documentId)
                        document.getElementById(collateralListId).innerHTML = response.Data;
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
                    $(modalId).modal('show');
                }
            },
            error: function (err) {
                ShowAlertToast(1, errorInConnectToServer, err);
            }
        });
    } catch (e) {
        ShowAlertToast(1, errorInProccessDataInServer, e);
    }
    return false;
}
submitPaymentModalFormAndUpdateTarget = (form, targetId, documentId, targetModalId) => {
    try {
        if (event != undefined)
            event.preventDefault();
        modalId = `#${targetModalId}`;
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != undefined) {
                    if (response.IsSuccess) {
                        getInstallments(documentId)
                        getInstllmentInfo(documentId)
                        //getCollateralsInfo(documentId)
                        $(modalId).modal('hide');
                    }
                    ShowAlertToast(response.Type, response.Title, response.Message);
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
function getInstallments(documentId) {
    $.ajax({
        type: "Get",
        url: '/Dashboard/Document/GetInstallmentList',
        data: { documentId: documentId },
        success: function (response) {
            if (response.IsSuccess) {
                document.getElementById('installments').innerHTML = response.Data;
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInProccessDataInServer, err);
        }
    });
}
function getInstllmentInfo(documentId) {
    $.ajax({
        type: "Get",
        url: '/Dashboard/Document/GetInstallmentInfo',
        data: { documentId: documentId },
        success: function (response) {
            if (response.IsSuccess) {
                document.getElementById('instllmentInfo').innerHTML = response.Data;
            } else {
                ShowAlertToast(response.Type, response.Title, response.Message);
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInProccessDataInServer, err);
        }
    });
}

function getCollateralsInfo(documentId) {
    $.ajax({
        type: "Get",
        url: '/Dashboard/Document/GetCollateralInfo',
        data: { documentId: documentId },
        success: function (response) {
            if (response.IsSuccess) {
                document.getElementById('collateralsInfo').innerHTML = response.Data;
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInProccessDataInServer, err);
        }
    });
}

function clear_form_createOrEditPayment() {
    document.getElementById('PersianPaymentDate').value = '';
    document.getElementById('PaymentAmount').value = '';
    document.getElementById('inputFile').value = '';
    document.getElementById('previewImg').src = '';
    document.getElementById('previewImg').style.display = 'none';
    document.getElementById('btnImgDel').style.display = 'none';
    document.getElementById('IsPayInstallment').checked = false;
    document.getElementById('DelayDay').value = '';
    document.getElementById('Description').value = '';
    document.getElementById('CustomerMessageContent').value = '';
    document.getElementById('PaymentId').value = '0';
    $("#PaymentType").val("").change();
    togleShowElement(document.getElementById('IsPayInstallment'), 'installmentOptions');
}

function clear_form_collateral() {
    $("#CollateralTypeId").val("").change();
    document.getElementById('CollateralId').value = '0';
    document.getElementById('Description').value = '';
    document.getElementById('inputFile1').value = '';
    document.getElementById('previewImg1').src = '';
    document.getElementById('previewImg1').style.display = 'none';
    document.getElementById('btnImgDel1').style.display = 'none';

}
function clear_form_gurantee() {
    $("#guranteeType").val("").change();
    document.getElementById('gurranteeId').value = '';
    document.getElementById('guranteeDescription').value = '';
    document.getElementById('guranteeFile').value = '';
    document.getElementById('guranteeImageFile').src = '';
}

 function  DelayCalculation() {
     try {
        debugger
        var installmentId = document.getElementById('InstallmentId').value;
        var paymentDate = document.getElementById('PersianPaymentDate').value;
        var selectedPaymentId = document.getElementById('PaymentId').value;
        let delayCalculationUrl = '/Dashboard/Document/CalculationInstallmentDelay';
        $.ajax({
            type: "Get",
            url: delayCalculationUrl,
            data: { installmentId: installmentId, paymentDate: paymentDate, selectedPaymentId: selectedPaymentId },
            success: function (response) {
                if (response != undefined) {
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
// function  fillPaymentDescriptionWithDelayDays(autoFillDelayDay = true) {

//     if (autoFillDelayDay)
//          DelayCalculation();
//   // setTimeout(fillPaymentDescription, 500);
//     fillPaymentDescription();
//}

function fillPaymentDescriptionWithDelayDays(autoFillDelayDay = true) {
    
    try {
        debugger
        var PaymentAmount = document.getElementById('PaymentAmount').value;
        var InstallmentId = document.getElementById('InstallmentId').value;
        var PaymentId = document.getElementById('PaymentId').value;
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
        debugger
        //var paymentType = document.getElementById("PaymentType");
        var amount = document.getElementById('PaymentAmount').value;
        var installmentId = document.getElementById('InstallmentId').value;
        var paymentId = document.getElementById('PaymentId').value;
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

function GetSettleMessage() {
    
    var documentId = document.getElementById('DocumentId').value;
    var settleDate = document.getElementById('SettleDate').value;
    var deliveryDate = document.getElementById('DeliveryDate').value;
    var returnedAmount = document.getElementById('ReturnedAmount').value;
    var discountAmount = document.getElementById('DiscountAmount').value;
    var url = `/Dashboard/Document/GenerateSettleMessage?documentId=${documentId}&settleDate=${settleDate}&deliveryDate=${deliveryDate}&returnedAmount=${returnedAmount}&discountAmount=${discountAmount}`;
    $.ajax({
        type: "Get",
        url: url,
        success: function (response) {
            
            if (response.IsSuccess) {
                if (document.getElementById('CustomerSettleMessageContent') != undefined) {
                    document.getElementById('CustomerSettleMessageContent').value = response.Data;
                }
            }
        },
        error: function (err) {
            ShowAlertToast(1, errorInProccessDataInServer, err);
        }
    });
    //try {


    //} catch (e) {
    //    ShowAlertToast(2, errorInConnectToServer, e);
    //}
    //return false;
}



//function validateDocumentDate(selectedDate='') {
//  selectedDateArrat=  selectedDate.split('/');
//}