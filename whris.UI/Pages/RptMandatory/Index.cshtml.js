//For Mandatory Report

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
        window.open(window.location.origin + "/RptMandatory/RepGovernmentMonthlyReport?paramCompanyId=" + $("#CompanyId").val() +
            "&paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val(), '_blank').focus();
    }
    else if($selectedReportId > 1 && $selectedReportId <= 10)
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

function onEntityComboboxChange()
{

}

function DateChange() {

}

