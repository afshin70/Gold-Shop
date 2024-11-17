function SelectOption(index) {
    ClearAllOption();
    $('div#op-' + index).addClass("bg-success");
}

function ClearAllOption() {
    for (var i = 1; i < 5; i++) {
        $('div#op-' + i).removeClass("bg-success").addClass("bg-primary");
    }
}