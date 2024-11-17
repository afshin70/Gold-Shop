function GetDbBak(bakType) {
    var trainingId = $("#TrainingId").val();
    swal({
        title: 'درخواست گرفتن بکاپ جدید به سرور داده شود؟',
        text: 'نوع بکاپ درخواستی: ' + bakType,
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#DD6B55',
        confirmButtonText: 'بله، ارسال درخواست !',
        cancelButtonText: 'نه، لغو درخواست !',
        closeOnConfirm: false,
        closeOnCancel: false,
        showLoaderOnConfirm: true
    },
        function (isConfirm) {
            if (isConfirm) {
                $.ajax({
                    url: '/System/GetDatabaseBackUp',
                    data: { backupType: bakType },
                    success: function (result) {
                        //swal({
                        //    title: 'موفق',
                        //    text: result.Message,
                        //    html: true,
                        //    confirmButtonText: 'بستن',
                        //    confirmButtonColor: '#52c466',
                        //    type: 'success'
                        //});
                        //ShowAlert(0, 'موفق', 'بکاپ درخواستی با موفقیت گرفته شد.');

                        if (result.Type === 0) {
                            //swal({
                            //    title: 'موفق',
                            //    text: result.Message,
                            //    html: true,
                            //    confirmButtonText: 'بستن',
                            //    confirmButtonColor: '#52c466',
                            //    type: 'success'
                            //});
                            ShowAlert(0, 'موفق', 'بکاپ درخواستی با موفقیت گرفته شد.');
                        }
                        else {
                            ShowAlert(1, 'خطا!!!', 'خطایی رخ داده است دوباره تلاش کنید.');
                            //swal({
                            //    title: 'ناموفق !',
                            //    text: result.Message,
                            //    html: true,
                            //    confirmButtonText: 'بستن',
                            //    confirmButtonColor: '#ff994c',
                            //    type: 'error'
                            //});
                        }
                    },
                    error: function (xhr, status, error) {
                        //switch (xhr.status) {
                        //    default:
                        //        ShowAlert(1, 'خطا!!!', 'خطایی رخ داده است دوباره تلاش کنید.');
                        //        break;
                        //}
                        ShowAlert(1, 'خطا!!!', 'خطایی رخ داده است دوباره تلاش کنید.');
                    },
                    type: "POST"
                });
            } else
                //swal('لغو عملیات', '»آیتم شما امن است.', 'error');
                swal({
                    title: 'لغو عملیات !',
                    text: 'آیتم شما امن است...',
                    html: true,
                    confirmButtonText: 'بستن',
                    confirmButtonColor: '#ff994c',
                    type: 'error'
                });
        });
}