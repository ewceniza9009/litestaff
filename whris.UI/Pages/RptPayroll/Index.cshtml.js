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
        window.open(window.location.origin + "/RptPayroll/RepPayslip?paramId=" + $("#PayrollId").val(), '_blank').focus();
    }

    if ($selectedReportId == 2) {
        window.open(window.location.origin + "/RptPayroll/RepPayslipLengthwise?paramId=" + $("#PayrollId").val() + "&paramEmploymentType=" + $("#EmploymentType").val(), '_blank').focus();
    }

    if ($selectedReportId == 2.1) {
        window.open(window.location.origin + "/RptPayroll/RepPayslipContinues?paramId=" + $("#PayrollId").val() + "&paramEmployeeId=" + $("#EmployeeId").val() + "&paramEmploymentType=" + $("#EmploymentType").val(), '_blank').focus();
    }

    if ($selectedReportId == 4) {
        window.open(window.location.origin + "/RptPayroll/RepPayrollWithHrs?paramId=" + $("#PayrollId").val() + "&paramEmploymentType=" + $("#EmploymentType").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }

    if ($selectedReportId == 5) {
        window.open(window.location.origin + "/RptPayroll/RepPayrollWithDepartments?paramId=" + $("#PayrollId").val() + "&paramEmploymentType=" + $("#EmploymentType").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val() + "&paramDepartmentId=" + $("#DepartmentId").val(), '_blank').focus();
    }

    if ($selectedReportId == 8) {
        window.open(window.location.origin + "/RptPayroll/RepPayrollOtherDeduction?paramId=" + $("#PayrollId").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }

    if ($selectedReportId == 9) {
        window.open(window.location.origin + "/RptPayroll/RepPayrollOtherIncome?paramId=" + $("#PayrollId").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
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

function onEntityComboboxChange() { }
