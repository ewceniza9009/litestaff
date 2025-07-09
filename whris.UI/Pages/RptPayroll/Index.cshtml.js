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

    if ($selectedReportId == 3.1) {
        const token = $('input[name="__RequestVerificationToken"]').val();

        const payrollId = $("#PayrollId").val();
        const employmentType = $("#EmploymentType").val();
        const companyId = $("#CompanyId").val();
        const branchId = $("#BranchId").val();

        const formData = new FormData();
        formData.append("ParamPayrollId", payrollId);
        formData.append("ParamEmploymentType", employmentType);
        formData.append("ParamCompanyId", companyId);
        formData.append("ParamBranchId", branchId);

        fetch('/RptPayroll/Index?handler=ExportToExcel', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                const disposition = response.headers.get('content-disposition');
                if (disposition && disposition.indexOf('attachment') !== -1) {
                    return response.blob();
                }
                return null;
            })
            .then(blob => {
                if (!blob) return;                             

                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.style.display = 'none';
                a.href = url;
                a.download = 'PayrollWorksheetWIncomeDeductionBreakdown.xlsx';                 
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                a.remove();
            })
            .catch(error => {
                console.error('Fetch error:', error);
            });
    }

    if ($selectedReportId == 4) {
        window.open(window.location.origin + "/RptPayroll/RepPayrollWithHrs?paramId=" + $("#PayrollId").val() + "&paramEmploymentType=" + $("#EmploymentType").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }

    if ($selectedReportId == 5) {
        window.open(window.location.origin + "/RptPayroll/RepPayrollWithDepartments?paramId=" + $("#PayrollId").val() + "&paramEmploymentType=" + $("#EmploymentType").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val() + "&paramDepartmentId=" + $("#DepartmentId").val(), '_blank').focus();
    }

    if ($selectedReportId == 6) {
        window.open(window.location.origin + "/RptPayroll/RepMonthlyPayroll?paramEmploymentType=" + $("#EmploymentType").val() + "&paramPayrollGroupId=" + $("#PayrollGroupId").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val() + "&paramMonthId=" + $("#MonthId").val() + "&paramPeriod=" + $("#Period").val(), '_blank').focus();
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
