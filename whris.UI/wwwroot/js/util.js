function GetFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}

function GetGridViewJsonData($data)
{
    //var dataView = $data.data().kendoGrid.dataSource.view();
    var dataView = $data.data().kendoGrid.dataSource._data;
    var jsonText = JSON.stringify(dataView);

    return JSON.parse(jsonText);
}

function GetGridViewJsonData2($data) {
    var dataView = $data.data().kendoGrid.dataSource._data;

    const replacer = (key, value) => {
        if (value instanceof Date) {
            return {
                __type: 'date',
                iso: value.toISOString(),
                timezone: value.getTimezoneOffset()
            };
        }
        return value;
    };

    var jsonText = JSON.stringify(dataView, replacer, 2);

    const reviver = (key, value) => {
        if (typeof value === 'object' && value !== null && '__type' in value && value.__type === 'date') {
            return new Date(value.iso);
        }
        return value;
    };

    return JSON.parse(jsonText, reviver);
}

function GetSaveMessage($label)
{
    var save = new jBox("Notice", {
        attributes: {
            x: "right",
            y: "bottom"
        },
        //stack: !1,
        animation: {
            open: "tada",
            close: "zoomIn"
        },
        delayOnHover: !0,
        showCountdown: !0,
        color: "blue",
        title: "WHRIS",
        content: "Record successfully saved for [" + $label + "]..."
    });

    save.open();
}

function GetPostMessage($label) {
    var save = new jBox("Notice", {
        attributes: {
            x: "right",
            y: "bottom"
        },
        //stack: !1,
        animation: {
            open: "tada",
            close: "zoomIn"
        },
        delayOnHover: !0,
        showCountdown: !0,
        color: "blue",
        title: "WHRIS",
        content: "Transaction #:  [" + $label + "] successfully posted..."
    });

    save.open();
}

function GetErrorMessage($error, $action)
{
    var message = "The current user doesn't have the rights to [" + $action + "] the function'.";
    var color = "red";

    if ($error.status == 500)
    {
        message = "The record cannot be deleted because it is referenced to another entity.";
        color = "yellow";
    }

    var messageBox = new jBox("Notice", {
        attributes: {
            x: "right",
            y: "bottom"
        },
        //stack: !1,
        animation: {
            open: "pulse",
            close: "zoomIn"
        },
        delayOnHover: !0,
        showCountdown: !0,
        color: color,
        title: "WHRIS",
        content: message
    });

    messageBox.open();
}

function GetWarningMessage($content)
{
    var messageBox = new jBox("Notice", {
        attributes: {
            x: "right",
            y: "bottom"
        },
        //stack: !1,
        animation: {
            open: "pulse",
            close: "zoomIn"
        },
        delayOnHover: !0,
        showCountdown: !0,
        color: "yellow",
        title: "WHRIS",
        content: $content
    });

    messageBox.open();
}

function FormatTime($date) {

    if ($date == null)
    {
        return null;
    }

    const newDate = new Date($date); 
    const hours = newDate.getHours();
    const minutes = newDate.getMinutes();
    const amOrPm = hours >= 12 ? 'PM' : 'AM';

    return `${hours % 12}:${minutes.toString().padStart(2, '0')} ${amOrPm}`;
}

function isFormDirty(e)
{
    //Enter	13
    //Up arrow	38
    //Down arrow	40
    //Left arrow	37
    //Right arrow	39
    //Escape	27
    //Spacebar	32
    //Ctrl	17
    //Alt	18
    //Tab	9
    //Shift	16
    //Caps - lock	20
    //Windows key	91
    //Windows option key	93
    //Backspace	8
    //Home	36
    //End	35
    //Insert	45
    //Delete	46
    //Page Up	33
    //Page Down	34
    //Numlock	144
    //F1 - F12	112 - 123
    //Print - screen ??
    //Scroll - lock	145
    //Pause -break	19

    var result = false;
    var excludedKeys = [13, 38, 40, 37, 39, 27, 32, 17, 18, 9, 16, 20, 91, 93, 8, 36, 35, 45, 46, 33, 34, 144, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 145, 19];

    for (var i = 0; i < excludedKeys.length; i++) {
        if (excludedKeys[i] != e.keyCode)
        {
            result = true;
            break;
        }
    }

    return result;
}

function cdbl($number)
{
    return parseFloat($number.replace(/,/g, ''))
}

function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function convertToTimeFormat(input) {
    input = input.trim();

    var formattedTime = input;

    if (input.search(":") == -1)
    {
        var hour = input.substr(0, 2);
        var minute = input.substr(2, 2);
        var period = input.substr(4);

        hour = hour.padStart(2, '0');
        minute = minute.padStart(2, '0');

        formattedTime = hour + ':' + minute + ' ' + period.toUpperCase(); 
    }   

    return formattedTime;
}

function detectMob() {
    const toMatch = [
        /Android/i,
        /webOS/i,
        /iPhone/i,
        /iPad/i,
        /iPod/i,
        /BlackBerry/i,
        /Windows Phone/i
    ];

    return toMatch.some((toMatchItem) => {
        return navigator.userAgent.match(toMatchItem);
    });
}

function applyMobileSizesToControls()
{
    if (detectMob())
    {
        document.getElementsByClassName("lookUpPanel")[0].style.display = "flex";
        document.getElementsByClassName("lookUpPanel")[0].style.cssText += "display: flex";
        document.getElementById("grid").style.width = "85vw";
    }
}

function applyMobileSizesToDetailGrid(gridName) {
    if (detectMob()) {
        document.getElementById(gridName).style.width = "85vw";
    }
}

function resizeTrnGrids()
{
    var showDrawer = localStorage.getItem("showDrawer");

    if (showDrawer == null)
    {
        showDrawer = "true";
    }

    if (document.getElementById("TrnLeaveApplicationLines")) {
        if (showDrawer == "true") {
            document.getElementById("TrnLeaveApplicationLines").style.width = "83vw";
        }
        else {
            document.getElementById("TrnLeaveApplicationLines").style.width = "95vw";
        }
    }

    if (document.getElementById("TrnOTApplicationLines")) {
        if (showDrawer == "true") {
            document.getElementById("TrnOTApplicationLines").style.width = "83vw";
        }
        else {
            document.getElementById("TrnOTApplicationLines").style.width = "95vw";
        }
    }

    if (document.getElementById("gridLoanPayments")) {
        if (showDrawer == "true") {
            document.getElementById("gridLoanPayments").style.width = "83vw";
        }
        else {
            document.getElementById("gridLoanPayments").style.width = "95vw";
        }
    }

    if (document.getElementById("TrnPayrollOtherIncomeLines")) {
        if (showDrawer == "true") {
            document.getElementById("TrnPayrollOtherIncomeLines").style.width = "83vw";
        }
        else {
            document.getElementById("TrnPayrollOtherIncomeLines").style.width = "95vw";
        }
    }

    if (document.getElementById("TrnPayrollOtherDeductionLines")) {
        if (showDrawer == "true") {
            document.getElementById("TrnPayrollOtherDeductionLines").style.width = "83vw";
        }
        else {
            document.getElementById("TrnPayrollOtherDeductionLines").style.width = "95vw";
        }
    }

    if (document.getElementById("TrnLastWithholdingTaxLines")) {
        if (showDrawer == "true") {
            document.getElementById("TrnLastWithholdingTaxLines").style.width = "83vw";
        }
        else {
            document.getElementById("TrnLastWithholdingTaxLines").style.width = "95vw";
        }
    }

    if (document.getElementById("TrnChangeShiftLines")) {
        if (showDrawer == "true") {
            document.getElementById("TrnChangeShiftLines").style.width = "83vw";
        }
        else {
            document.getElementById("TrnChangeShiftLines").style.width = "95vw";
        }
    }
}