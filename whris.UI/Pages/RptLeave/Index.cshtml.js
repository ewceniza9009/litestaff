//For Leave Report

$selectedReportId = 0;

$("#DateStart").val(new Date().toLocaleDateString());
$("#DateEnd").val(new Date().toLocaleDateString());

function CmdHome() {
    window.open(window.location.origin).focus();
}

function CmdPreview()
{
    if ($selectedReportId == 1)
    {
        window.open(window.location.origin + "/RptLeave/RepLeaveSummary?paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val(), '_blank').focus();
    }

    if ($selectedReportId == 2) {
        window.open(window.location.origin + "/RptLeave/RepLeaveDetail?paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val(), '_blank').focus();
    }


    if ($selectedReportId == 4) {
        window.open(window.location.origin + "/RptLeave/RepLeaveLedger?paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val() +
            "&paramEmployeeId=" + $("#EmployeeId").val(), '_blank').focus();
    }

    if ($selectedReportId == 5) {
        window.open(window.location.origin + "/RptLeave/RepLeaveLedgerSummary?paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val(), '_blank').focus();
    }
}

function ListBoxChange(e)
{
    var element = e.sender.select();
    var dataItem = e.sender.dataItem(element[0]);

    $selectedReportId = dataItem.Value;
}

function DateChange()
{

}

function onEntityComboboxChange(e)
{

}
