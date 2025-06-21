//For Withholding Tax Report

$selectedReportId = 0;

function CmdHome() {
    window.open(window.location.origin).focus();
}

function CmdPreview()
{
    if ($selectedReportId == 1)
    {
        //window.open(window.location.origin + "/RptWithTax/RepLoanSummary?paramDateStart=" + $("#DateStart").val() +
        //    "&paramDateEnd=" + $("#DateEnd").val(), '_blank').focus();
    }

    alert("We apologize, The report selected is under developement for now.")
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
