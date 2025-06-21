//For Mandatory Report

$selectedReportId = 0;

function CmdHome() {
    window.open(window.location.origin).focus();
}

function CmdPreview()
{
    if ($selectedReportId == 1) {
        window.open(window.location.origin + "/RptATMBank/RepATMReport?paramPayrollId=" + $("#PayrollId").val() +
            "&paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val() + 
            "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }
    else if ($selectedReportId == 2) {
        window.open(window.location.origin + "/RptATMBank/RepNoneATMReport?paramPayrollId=" + $("#PayrollId").val() +
            "&paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val() +
            "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }
    else
    {
        alert("We apologize, The report selected is under developement for now.")
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

function onEntityComboboxChange()
{

}
