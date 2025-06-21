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
        window.open(window.location.origin + "/RptLoans/RepLoanSummary?paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val(), '_blank').focus();
    }

    if ($selectedReportId == 2) {
        window.open(window.location.origin + "/RptLoans/RepLoanLedger?paramEmployeeId=" + $("#EmployeeId").val() +
            "&paramLoanId=" + $("#LoanId").val(), '_blank').focus();
    }

    if ($selectedReportId == 2.1) {
        window.open(window.location.origin + "/RptLoans/RepLoanCrossTab?paramDateStart=" + $("#DateStart").val() +
            "&paramDateEnd=" + $("#DateEnd").val(), '_blank').focus();
    }

    if ($selectedReportId == 4) {
        window.open(window.location.origin + "/RptLoans/RepLoanDeduction?paramPayrollId=" + $("#PayrollId").val(), '_blank').focus();
    }

    if ($selectedReportId == 6) {
        window.open(window.location.origin + "/RptLoans/RepSSSLoanCoverLetter?paramPeriodId=" + $("#PeriodId").val() + 
            "&paramMonthId=" + $("#MonthId").val() +
            "&paramCompanyId=" + $("#CompanyId").val() +
            "&paramReceiptNumber=" + $("#ReceiptNumber").val() +
            "&paramReceiptDate=" + $("#ReceiptDate").val() +
            "&paramReceiptAmount=" + $("#ReceiptAmount").val() +
            "&paramFileName=" + $("#FileName").val(), '_blank').focus();
    }

    if ($selectedReportId == 7) {
        window.open(window.location.origin + "/RptLoans/RepSSSLoansReport?paramPeriodId=" + $("#PeriodId").val() +
            "&paramMonthId=" + $("#MonthId").val() +
            "&paramCompanyId=" + $("#CompanyId").val(), '_blank').focus();
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

function onLoanIdOpen() {
    var comboBox = $("#LoanId").getKendoComboBox();
    var empId = $("#EmployeeId").val()

    if (empId != '')
    {
        comboBox.dataSource.filter({ field: "EmployeeId", operator: "eq", value: parseInt(empId) });
    }
    else
    {
        comboBox.dataSource.filter({ });
    }
}

