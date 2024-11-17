function timeValidate(control) {
    if (control.value === '__' + control.value.charAt(2) + '__')
        return;
    control.value = GetCorrectTime(control.value);
    var str = control.value.split(control.value.charAt(2));
    if (str != "") {
        var hour = parseFloat(str[0]);
        var minute = parseFloat(str[1]);
        if (hour > 23 || minute > 59 || hour < 0 || minute < 0 || str.toString().length < 5) {
            alert('ساعت معتبر نمی باشد');
            control.value = '';
          /*  $('.time').mask('00:00');*/
            control.focus();
        }
    }
}
function timeMask() {
    $('.time').mask('00:00');
    
    /*$(id).mask('00:00');*/
}
$(document).ready(function () {
    $('.time').mask('00:00');
});
function GetCorrectTime(t) {
    var result = '';
    for (var i = 0; i < 5; i++) {
        if (t.toString().charAt(i) === '_')
            result = result + '0';
        else
            result = result + t.toString().charAt(i);
    }
    return result;
}