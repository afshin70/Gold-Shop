function AddEmptyItem(id) {
  $(id).append($('<option></option>').attr("value", "").text("انتخاب نمایید"));
  $(id).val("").change();
}

//$(function () {
//    alert(1);
//    console.log("ready!");
//});

//$(document).ready(function () {
//    alert(1);
//    console.log("ready!");
//});

$('#ClusterId').change(function () {
    $('#FieldId').empty();
    $('#ProfessionId').empty();
    $('#AbilityId').empty();
    if ($(this).val() !== undefined && $(this).val() !== null && $(this).val() !== '') {
        jQuery.getJSON('/Fields/GetAll', { id: $(this).val() }, function (data) {
            jQuery.each(data, function (i) {
                var option = $('<option></option>').attr("value", data[i].Id).text(data[i].Name);
                $("#FieldId").append(option);
            });
            AddEmptyItem("#FieldId");
            AddEmptyItem("#ProfessionId");
            AddEmptyItem("#AbilityId");
        });
    }
});

$('#FieldId').change(function () {
    $('#ProfessionId').empty();
    $('#AbilityId').empty();
    if ($(this).val() !== undefined && $(this).val() !== null && $(this).val() !== '') {
        jQuery.getJSON('/Professions/GetAll', { id: $(this).val() }, function (data) {
            jQuery.each(data, function (i) {
                var option = $('<option></option>').attr("value", data[i].Id).text(data[i].Name);
                $("#ProfessionId").append(option);
            });
            AddEmptyItem("#ProfessionId");
            AddEmptyItem("#AbilityId");
        });
    }
});

$('#ProfessionId').change(function () {
    $('#AbilityId').empty();
    if ($(this).val() !== undefined && $(this).val() !== null && $(this).val() !== '') {
        jQuery.getJSON('/Abilities/GetAll', { id: $(this).val() }, function (data) {
            jQuery.each(data, function (i) {
                var option = $('<option></option>').attr("value", data[i].Id).text(data[i].Name);
                $("#AbilityId").append(option);
            });
            AddEmptyItem("#AbilityId");
        });
    }
});