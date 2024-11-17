$(function () {
    $('#ProvinceId').change(function () {
        $('#CityId').empty();
        $('#BranchId').empty();
        if ($(this).val() !== undefined && $(this).val() !== null && $(this).val() !== '') {
            jQuery.getJSON('/Cities/GetAll', { id: $(this).val() }, function (data) {
                jQuery.each(data, function (i) {
                    var option = $('<option></option>').attr("value", data[i].Id).text(data[i].Name);
                    $("#CityId").append(option);
                });
                $("#CityId").append($('<option></option>').attr("value", "").text("شهر خود را انتخاب کنید..."));
                $("#CityId").val("").change();
                $("#BranchId").append($('<option></option>').attr("value", "").text("شعبه خود را انتخاب کنید..."));
                $("#BranchId").val("").change();
            });
        }
    });
});