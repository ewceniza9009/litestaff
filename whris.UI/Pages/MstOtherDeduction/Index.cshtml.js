//For Other Deduction
$transitionTime = 200;

var grid = new Object();

$isDirty = false;
$isSaving = false;

window.addEventListener('beforeunload', function (e) {
    if ($isDirty) {
        e.preventDefault();
        e.returnValue = '';
    }
});

$(document).ready(function () {

});

//Button Commands and Lookup
function CmdSave()
{
    var frmDataSF1 = GetGridViewJsonData($("#MstOtherDeductions"));
    var dataValue = { ...frmDataSF1 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $isSaving = true

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/MstOtherDeduction/Index?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "otherDeductions": dataValue
        },
        success: function (data) {
            GetSaveMessage("Other Deductions");

            $isDirty = false;
        },
        error: function (error) {
            GetErrorMessage(error, "Save");

            $isDirty = false;
        }
    });
}

function CmdHome() {
    window.open(window.location.origin).focus();
}

//Other Deduction
function CmdAddOtherDeduction() {
    $isDirty = true;

    $("#MstOtherDeductions").getKendoGrid().dataSource.insert();
}

function CmdDeleteOtherDeduction(e) {
    $isDirty = true;

    var subGrid = $("#MstOtherDeductions").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function onCheckBoxChange()
{
    $isDirty = true;
}