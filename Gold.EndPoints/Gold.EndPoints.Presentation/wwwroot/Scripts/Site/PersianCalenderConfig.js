$(document).ready(function () {
    let config = {
        "inline": false,
        //format: 'YYYY/MM/DD HH:mm',
        format: 'YYYY/MM/DD',
        "viewMode": "day",
        "autoClose": true,
        "position": "auto",
        "calendarType": "persian",
        "observer": false,
        "calendar": {
            "persian": {
                "locale": "fa",
                "showHint": false,
                "leapYearMode": "algorithmic"
            },
            "gregorian": {
                "locale": "en",
                "showHint": false
            }
        },
        "navigator": {
            "enabled": true,
            "scroll": {
                "enabled": false
            },
        },
        "toolbox": {
            "enabled": true,
            "calendarSwitch": {
                "enabled": false,
                "format": "MMMM"
            },
            "todayButton": {
                "enabled": true,
                "text": {
                    "fa": "امروز",
                    "en": "Today"
                }
            },
            "submitButton": {
                "enabled": true,
                "text": {
                    "fa": "تایید",
                    "en": "Submit"
                }
            },
            "text": {
                "btnToday": "امروز"
            }
        },
        "timePicker": {
            "enabled": false,
            "step": 1,
            "hour": {
                "enabled": true,
                "step": null
            },
            "minute": {
                "enabled": true,
                "step": null
            },
            "second": {
                "enabled": false,
                "step": null
            },
            "meridian": {
                "enabled": false
            }
        },
        "dayPicker": {
            "enabled": true,
            "titleFormat": "YYYY MMMM"
        },
        "monthPicker": {
            "enabled": true,
            "titleFormat": "YYYY"
        },
        "yearPicker": {
            "enabled": true,
            "titleFormat": "YYYY"
        },
        "responsive": true,
    };

    $(".datePicker").pDatepicker({
        ...config,
    });
});

$(document).ready(function () {
    let config = {
        "inline": false,
        //format: 'YYYY/MM/DD HH:mm',
        format: 'HH:mm',
        "viewMode": "day",
        "autoClose": true,
        "position": "auto",
        "calendarType": "persian",
        "observer": false,
        "calendar": {
            "persian": {
                "locale": "fa",
                "showHint": false,
                "leapYearMode": "algorithmic"
            },
            "gregorian": {
                "locale": "en",
                "showHint": false
            }
        },
        "navigator": {
            "enabled": false,
            "scroll": {
                "enabled": false
            },
        },
        "toolbox": {
            "enabled": false,
            "calendarSwitch": {
                "enabled": false,
                "format": "MMMM"
            },
            "todayButton": {
                "enabled": false,
                "text": {
                    "fa": "امروز",
                    "en": "Today"
                }
            },
            "submitButton": {
                "enabled": true,
                "text": {
                    "fa": "تایید",
                    "en": "Submit"
                }
            },
            "text": {
                "btnToday": "امروز"
            }
        },
        "timePicker": {
            "enabled": true,
            "step": 1,
            "hour": {
                "enabled": true,
                "step": null
            },
            "minute": {
                "enabled": true,
                "step": null
            },
            "second": {
                "enabled": false,
                "step": null
            },
            "meridian": {
                "enabled": false
            }
        },
        "dayPicker": {
            "enabled": false,
            "titleFormat": "YYYY MMMM"
        },
        "monthPicker": {
            "enabled": false,
            "titleFormat": "YYYY"
        },
        "yearPicker": {
            "enabled": false,
            "titleFormat": "YYYY"
        },
        "responsive": true,
    };

    $(".timePicker").pDatepicker({
        ...config,
    });
});