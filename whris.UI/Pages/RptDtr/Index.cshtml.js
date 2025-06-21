//For Dtr Report

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
        window.open(window.location.origin + "/RptDtr/RepTardiness?paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }

    if ($selectedReportId == 1.5) {
        window.open(window.location.origin + "/RptDtr/RepTardinessSummary?paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }

    if ($selectedReportId == 2) {
        window.open(window.location.origin + "/RptDtr/RepAbsences?paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }

    if ($selectedReportId == 4) {
        window.open(window.location.origin + "/RptDtr/RepAllowance?paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
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
