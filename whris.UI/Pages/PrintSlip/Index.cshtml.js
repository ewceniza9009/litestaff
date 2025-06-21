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
        window.open(window.location.origin + "/PrintSlip/RepPayslip?paramId=" + $("#PayrollId").val(), '_blank').focus();
    }

    if ($selectedReportId == 2) {
        window.open(window.location.origin + "/PrintSlip/RepPayslipLengthwise?paramId=" + $("#PayrollId").val() + "&paramEmploymentType=" + $("#EmploymentType").val(), '_blank').focus();
    }

    if ($selectedReportId == 2.1) {
        window.open(window.location.origin + "/PrintSlip/RepPayslipContinues?paramId=" + $("#PayrollId").val() + "&paramEmployeeId=" + $("#EmployeeId").val() + "&paramEmploymentType=" + $("#EmploymentType").val(), '_blank').focus();
    }

    if ($selectedReportId == 4) {
        window.open(window.location.origin + "/PrintSlip/RepPayrollWithHrs?paramId=" + $("#PayrollId").val() + "&paramEmploymentType=" + $("#EmploymentType").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }

    if ($selectedReportId == 5) {
        window.open(window.location.origin + "/PrintSlip/RepPayrollWithDepartments?paramId=" + $("#PayrollId").val() + "&paramEmploymentType=" + $("#EmploymentType").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }

    if ($selectedReportId == 8) {
        window.open(window.location.origin + "/PrintSlip/RepPayrollOtherDeduction?paramId=" + $("#PayrollId").val(), '_blank').focus();
    }

    if ($selectedReportId == 9) {
        window.open(window.location.origin + "/PrintSlip/RepPayrollOtherIncome?paramId=" + $("#PayrollId").val(), '_blank').focus();
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
