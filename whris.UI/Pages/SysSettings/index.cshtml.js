$isDirty = false;

window.addEventListener('beforeunload', function (e) {
    if ($isDirty) {
        e.preventDefault();
        e.returnValue = '';
    }
});

function onEntityComboboxChange(e)
{
    $isDirty = true;
}

function onCheckboxChange(e)
{
    $isDirty = true;
}

function CmdSave()
{
    localStorage.setItem("CurrentPeriodId", $("#CurrentPeriodId").val());
    localStorage.setItem("DefualtOvertimeLimitHours", $("#DefualtOvertimeLimitHours").val());
    localStorage.setItem("IncludeTimeInOT", $("#IncludeTimeInOT")[0].checked);

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
        content: "Record successfully saved..."
    });

    save.open();

    $isDirty = false;

    var delayInMilliseconds = 1000; //1 second

    setTimeout(function () {
        //your code to be executed after 1 second
    }, delayInMilliseconds);

    location.reload();
}

var currentPeriodId = localStorage.getItem("CurrentPeriodId");
var defualtOvertimeLimitHours = localStorage.getItem("DefualtOvertimeLimitHours");
var includeTimeInOT = localStorage.getItem("IncludeTimeInOT");

if (currentPeriodId == null)
{
    localStorage.setItem("CurrentPeriodId", 7);
    currentPeriodId = localStorage.getItem("CurrentPeriodId");   
}

if (defualtOvertimeLimitHours == null)
{
    localStorage.setItem("DefualtOvertimeLimitHours", 1.00);
    defualtOvertimeLimitHours = localStorage.getItem("DefualtOvertimeLimitHours");
}

if (includeTimeInOT == null)
{
    localStorage.setItem("IncludeTimeInOT", false);
    includeTimeInOT = localStorage.getItem("IncludeTimeInOT");
}

$("#CurrentPeriodId").val(currentPeriodId);
$("#DefualtOvertimeLimitHours").val(defualtOvertimeLimitHours);
$("#IncludeTimeInOT").prop("checked", includeTimeInOT == "true" );