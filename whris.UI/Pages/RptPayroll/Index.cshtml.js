$selectedReportId = 0;

var currentYear = new Date().getFullYear();
$("#DateStart").val(new Date(currentYear, 0, 1).toLocaleDateString());
$("#DateEnd").val(new Date(currentYear, 11, 31).toLocaleDateString());

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

    if ($selectedReportId == 3.3) {
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

        fetch('/RptPayroll/Index?handler=ExportToExcelLegacy', {
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
                a.download = 'PayrollWorksheetWIncomeDeductionBreakdown_Legacy.xlsx';
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
        window.open(window.location.origin + "/RptPayroll/RepMonthlyPayroll?paramEmploymentType=" + $("#EmploymentType").val() + "&paramPayrollGroupId=" + $("#PayrollGroupId").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val() + "&paramMonthId=" + $("#MonthId").val() + "&paramPeriod=0&paramDepartmentId=" + $("#DepartmentId").val() + "&paramEmployeeId=" + $("#EmployeeId").val(), '_blank').focus();
    }

    if ($selectedReportId == 7.1) {
        exportMonthlySummaryExcel("ExportMonthlyRateToExcel", "MonthlyRateSummaryReport.xlsx");
    }

    if ($selectedReportId == 7.2) {
        exportMonthlySummaryExcel("ExportActualMonthlySalaryToExcel", "ActualMonthlySalaryReport_Net.xlsx");
    }

    if ($selectedReportId == 9) {
        window.open(window.location.origin + "/RptPayroll/RepPayrollOtherDeduction?paramId=" + $("#PayrollId").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }

    if ($selectedReportId == 10) {
        window.open(window.location.origin + "/RptPayroll/RepPayrollOtherIncome?paramId=" + $("#PayrollId").val() + "&paramCompanyId=" + $("#CompanyId").val() + "&paramBranchId=" + $("#BranchId").val(), '_blank').focus();
    }

    if ($selectedReportId == 12) {
        window.open(window.location.origin + "/RptPayroll/Rep13thMonthPay?paramStartPayNo=" + $("#PayrollId").val() + "&ParamEndPayNo=" + $("#EndPayNo").val() + "&ParamEmployeeId=" + $("#EmployeeId").val() + "&ParamPayrollGroupId=" + $("#PayrollGroupId").val() + "&ParamCompanyId=" + $("#CompanyId").val() + "&ParamBranchId=" + $("#BranchId").val() + "&ParamDepartmentId=" + $("#DepartmentId").val(), '_blank').focus();
    }

    if ($selectedReportId == 13) {
        window.open(window.location.origin + "/RptPayroll/Rep13thMonthPaySummary?paramStartPayNo=" + $("#PayrollId").val() + "&ParamEndPayNo=" + $("#EndPayNo").val() + "&ParamEmployeeId=" + $("#EmployeeId").val() + "&ParamPayrollGroupId=" + $("#PayrollGroupId").val() + "&ParamCompanyId=" + $("#CompanyId").val() + "&ParamBranchId=" + $("#BranchId").val() + "&ParamDepartmentId=" + $("#DepartmentId").val(), '_blank').focus();
    }
}

function exportMonthlySummaryExcel(handlerName, defaultFilename) {
    const token = $('input[name="__RequestVerificationToken"]').val();

    const formData = new FormData();
    formData.append("ParamDateStart", $("#DateStart").val() || "");
    formData.append("ParamDateEnd", $("#DateEnd").val() || "");
    formData.append("ParamStartMonthId", $("#MonthId").val() || 0);
    formData.append("ParamEndMonthId", $("#EndMonthId").val() || 0);
    formData.append("ParamDepartmentId", $("#DepartmentId").val() || 0);
    formData.append("ParamPositionId", $("#PositionId").val() || 0);
    formData.append("ParamCompanyId", $("#CompanyId").val() || 0);
    formData.append("ParamBranchId", $("#BranchId").val() || 0);
    formData.append("ParamPayrollGroupId", $("#PayrollGroupId").val() || 0);
    formData.append("ParamEmploymentType", $("#EmploymentType").val() || "");
    formData.append("ParamEmployeeId", $("#EmployeeId").val() || 0);

    fetch('/RptPayroll/Index?handler=' + handlerName, {
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
            a.download = defaultFilename;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();
        })
        .catch(error => {
            console.error('Fetch error:', error);
        });
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
