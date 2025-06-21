//For Demographics Report

$selectedReportId = 0;

function CmdHome() {
    window.open(window.location.origin).focus();
}

function CmdPreview()
{
    if ($selectedReportId == 1)
    {
        window.open(window.location.origin + "/RptDemographics/RepEmployeeList?paramPayrollGroupId=" + $("#PayrollGroupId").val(), '_blank').focus();
    }

    if ($selectedReportId == 2) {
        window.open(window.location.origin + "/RptDemographics/RepEmployeeShifts?paramPayrollGroupId=" + $("#PayrollGroupId").val(), '_blank').focus();
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
