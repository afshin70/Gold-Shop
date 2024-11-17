
let tab = 0;
let dropdown = false;
$(document).on('keydown', function (e) {
    if (e.keyCode == 13) {
        tab++;
        $(`*[data-tab=${tab}]`).focus();
        if ($(`*[data-tab=${tab}]`)[0].type == "select-one") {
            if (!dropdown) {
                dropdown = true;
                tab--;
            } else {
                dropdown = false;
            }
        }
    }
});
