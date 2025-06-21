//For Payroll
$transitionTime = 200;

var grid = new Object();

$indexView = $(".indexView");
$detailView = $(".detailView");

$isDirty = false;
$isSaving = false;
$isTurnPage = false;
$isSearchFocused = false;

$SelectedNavigationControl = "none";
$SelectedAction = "none";

$SelectedGridRecord = new Object();

$employees = null;

window.addEventListener('beforeunload', function (e) {
    if ($isDirty) {
        e.preventDefault();
        e.returnValue = '';
    }
});

$(document).ready(function () {
    grid = $("#grid").data("kendoGrid");

    $("#loading").hide();

    applyMobileSizesToControls();
});

function onDetailGridDataBound() {
    applyMobileSizesToDetailGrid("TrnPayrollLines")


    $("#TrnPayrollLinesMobile").hide();

    if (detectMob()) {
        $("#TrnPayrollLinesMobile").hide();
        $("#TrnPayrollLinesMobile").show();
    }
}

function forgeryToken() {
    return {
        __RequestVerificationToken: kendo.antiForgeryTokens().__RequestVerificationToken,
        payrollGroupId: $("#payrollGroupCmb").val()
    };
}

//Button Commands and Lookup
function onPayrollGroupChange() {
    backToMainGrid();

    grid.dataSource.read();
}

function onClearPayrollGroupClick() {
    backToMainGrid();

    $("#payrollGroupCmb").val("");
    $("#payrollGroupCmb").data("kendoComboBox").text("");

    grid.dataSource.read();
}

function backToMainGrid() {
    $("#loading").hide();

    $SelectedNavigationControl = "lookup";
    $isSearchFocused = true;

    if ($isDirty) {
        $("#confirmSaveDialog").data("kendoDialog").open();
        $("#showDialogBtn").fadeOut();
    }

    if (!$isDirty) {
        $indexView.show($transitionTime);
        $detailView.hide($transitionTime);
    }
}

function onGridDatabound(e)
{
    if ($isSaving == true) {
        $detailView.removeAttr("hidden");

        //loadPartialView(grid.dataSource.view()[0].Id);

        $isSaving = false;

        return;
    }    
}

function onCancelSave()
{
    $isDirty = false;

    if ($SelectedNavigationControl == "lookup")
    {
        $indexView.show($transitionTime);
        $detailView.hide($transitionTime);
    }

    if ($SelectedNavigationControl == "turnPage")
    {
        loadPartialViewViaTurnPage();
    }
}

function onConfirmSave()
{
    CmdSave();

    if ($SelectedNavigationControl == "lookup")
    {
        $indexView.show($transitionTime);
        $detailView.hide($transitionTime);
    }

    if ($SelectedNavigationControl == "turnPage")
    {
        loadPartialViewViaTurnPage();
    }
}

function onCancelQuickChange()
{

}

function onConfirmQuickChange()
{
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnPayroll/Detail?handler=QuickChangeShift",
        data: {
            __RequestVerificationToken: token,
            "dtrId": $("#Id").val()
        },
        success: function (data) {
            $("#editDetailModal").modal("hide");

            GetPostMessage($("#Payrollnumber").val());

            loadPartialView($("#Id").val())
        },
        error: function (xhr, status, error) {
            GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
        }
    });
}

function CmdAdd()
{
    $indexView.hide($transitionTime);
    $detailView.show($transitionTime);

    $detailView.removeAttr("hidden");

    loadPartialView(0);

    $SelectedGridRecord = null;
}

function CmdHome()
{
    window.open(window.location.origin).focus();
}

function CmdDetail(e) {
    e.preventDefault();

    var item = grid.dataItem($(e.target).closest("tr"));

    $indexView.hide($transitionTime);
    $detailView.show($transitionTime);

    $detailView.removeAttr("hidden");

    $SelectedGridRecord = item;

    loadPartialView(item.Id);
}

function CmdPreviousPage() {
    $SelectedAction = "prev";

    turnPage()
}

function CmdNextPage() {
    $SelectedAction = "next";

    turnPage();
}

function CmdPreview() {
    if ($("#Id").val() == 0) {
        GetWarningMessage("Please save the record first before printing.");
    }
    else {
        window.open(window.location.origin + "/TrnPayroll/PayrollReport?paramId=" + $("#Id").val(), '_blank').focus();
    }
}

function CmdDelete()
{
    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnPayroll/Detail?handler=Delete",
        data:
        {
            __RequestVerificationToken: token,
            "Id": $("#Id").val()
        },
        success: function (data) {
            GetWarningMessage("Record [" + $("#PayrollNumber").val() + "] is successfully deleted.");

            $indexView.show($transitionTime);
            $detailView.hide($transitionTime);

            $isDirty = false;

            grid.dataSource.read();
        },
        error: function (error)
        {
            GetErrorMessage(error, "Delete");
        }
    });
}

function CmdSave()
{
    var frmData = GetFormData($("#frmDetail"));

    frmData.PeriodId = $("#PeriodId").val();
    frmData.Payrollnumber = $("#PayrollNumber").val();
    frmData.PayrollGroupId = $("#PayrollGroupId").val();

    frmData.IsLocked = $("#IsLocked")[0].checked;
    frmData.IsApproved = $("#IsApproved")[0].checked;

    if (frmData.IsLocked == false) {
        var response = confirm("Do you want to lock(YES) the record or just leave(Cancel) it for later?.");

        if (response) {
            frmData.IsLocked = true;
        }
    }

    var frmDataSF1 = { TrnPayrollLines: GetGridViewJsonData($("#TrnPayrollLines")) };

    var dataValue = { ...frmData, ...frmDataSF1 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnPayroll/Detail?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "payroll": dataValue
        },
        success: function (data) {
            GetSaveMessage($("#PayrollNumber").val());

            $isDirty = false;
            $isSaving = true

            //if ($isTurnPage != true)
            //{
            //    if ($SelectedGridRecord == null)
            //    {
            //        loadPartialView(0);
            //    }
            //    else
            //    {
            //        loadPartialView($SelectedGridRecord.Id);
            //    }

            //    grid.dataSource.read();
            //}

            loadPartialView(data);
            grid.dataSource.read();

            $isTurnPage = false;
        },
        error: function (error) {
            GetErrorMessage(error, "Save");

            $isDirty = false;
            $isSaving = true
        }
    });
}

function CmdBack()
{
    $SelectedNavigationControl = "lookup";

    if ($isDirty) {
        $("#confirmSaveDialog").data("kendoDialog").open();
        $("#showDialogBtn").fadeOut();
    }

    if (!$isDirty) {
        $indexView.show($transitionTime);
        $detailView.hide($transitionTime);
    }
}

//Payroll Line Grid
function CmdAddPayrollLine(e)
{
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/TrnPayroll/Detail?handler=AddPayrollLine",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            payrollId: $("#Id").val()
        },
        success: function (data) {
            data.EmployeeId = null;

            $("#TrnPayrollLines").getKendoGrid()
                .dataSource.insert(data);
        }
    });
}

function CmdDeleteAllLines(e) {
    $isDirty = true;

    var subGrid = $("#TrnPayrollLines").getKendoGrid();
    var subGridData = $("#TrnPayrollLines").getKendoGrid().dataSource.data();

    for (let i = 0; i < subGridData.length; i++) {
        subGridData[i].set("IsDeleted", true);
    }

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function CmdDeletePayrollLine(e)
{
    $isDirty = true;

    var subGrid = $("#TrnPayrollLines").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Show Modals
function CmdShowProcessDtr() {
    if ($("#Dtrid").val() > 0)
    {
        loadPartialViewProcessDtr();

        $("#processDtrDetailModal").modal("show");
    }
}

function CmdShowProcessOtherIncome()
{
    if ($("#PayrollOtherIncomeId").val() > 0) {
        loadPartialViewProcessOtherIncome();

        $("#processOtherIncomeDetailModal").modal("show");
    }
}

function CmdShowProcessOtherDeduction() {
    if ($("#PayrollOtherDeductionId").val() > 0) {
        loadPartialViewProcessOtherDeduction();

        $("#processOtherDeductionDetailModal").modal("show");
    }
}

function CmdShowMandatory()
{
    loadPartialViewProcessMandatory();

    $("#processMandatoryDetailModal").modal("show");
}

function CmdShowProcessWithholding()
{
    loadPartialViewProcessWithholding();

    $("#processWithholdingDetailModal").modal("show");
}

function CmdShowProcessTotals()
{
    loadPartialViewProcessTotals();

    $("#processTotalsDetailModal").modal("show");
}

function CmdExpandSalary(e)
{
    var subGrid = $("#TrnPayrollLines").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    loadPartialViewExpandSalary(item.get("Id"));

    $("#expandSalaryDetailModal").modal("show");
}

function CmdExpandOtherIncome(e)
{
    var subGrid = $("#TrnPayrollLines").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    loadPartialViewExpandPOI(item.get("Id"));

    $("#expandPOIDetailModal").modal("show");
}

function CmdExpandOtherIncomeNonTax(e)
{
    var subGrid = $("#TrnPayrollLines").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    loadPartialViewExpandPOIN(item.get("Id"));

    $("#expandPOINDetailModal").modal("show");
}

function CmdExpandOtherDeduction(e)
{
    var subGrid = $("#TrnPayrollLines").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    loadPartialViewExpandPOD(item.get("Id"));

    $("#expandPODDetailModal").modal("show");
}

//Process Dtr
function CmdProcessDtr() {
    if ($("#Id").val() > 0) {
        $("#btnPayrollProcessSpin").removeAttr("hidden")

        $("#btnPayrollProcessLock").hide();
        $("#btnPayrollProcessSpin").show();

        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnPayroll/Detail?handler=ProcessDtr",
            data: {
                __RequestVerificationToken: token,
                "payrollId": $("#Id").val(),
                "payrollGroupId": $("#PayrollGroupId").val(),
                "dtrId": $("#Dtrid").val(),
                "employeeId": $("#ProcessDtrEmployeeId").val()
            },
            success: function (data) {
                $("#processDtrDetailModal").modal("hide");

                $("#btnPayrollProcessLock").show();
                $("#btnPayrollProcessSpin").hide();

                GetPostMessage($("#PayrollNumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                $("#btnPayrollProcessLock").show();
                $("#btnPayrollProcessSpin").hide();

                if (status == "error")
                {
                    const importantParts = xhr.responseText.split("\n");

                    GetWarningMessage("Error occured, " + importantParts[0]);

                    $("#processDtrDetailModal").modal("hide");
                }
                else
                {
                    GetWarningMessage("Error occured, " + "It's either server is unresponsive or there was no file uploaded.");
                }                
            }
        });
    }
}

//Process Other Income
function CmdProcessOtherIncome()
{
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnPayroll/Detail?handler=ProcessOtherIncome",
            data: {
                __RequestVerificationToken: token,
                "payrollId": $("#Id").val(),
                "payrollOtherIncomeId": $("#PayrollOtherIncomeId").val(),
                "employeeId": $("#ProcessOtherIncomeEmployeeId").val()
            },
            success: function (data) {
                $("#processOtherIncomeDetailModal").modal("hide");

                GetPostMessage($("#PayrollNumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
            }
        });
    }
}

//Process Other Deduction
function CmdProcessOtherDeduction() {
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnPayroll/Detail?handler=ProcessOtherDeduction",
            data: {
                __RequestVerificationToken: token,
                "payrollId": $("#Id").val(),
                "payrollOtherDeductionId": $("#PayrollOtherDeductionId").val(),
                "employeeId": $("#ProcessOtherDeductionEmployeeId").val()
            },
            success: function (data) {
                $("#processOtherDeductionDetailModal").modal("hide");

                GetPostMessage($("#PayrollNumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
            }
        });
    }
}

//Process Mandatory
function CmdProcessMandatory() {
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        var type = 1;

        if ($("#ProcessMandatoryPHIC")[0].checked == true) {
            type = 2;
        }

        if ($("#ProcessMandatoryHDMF")[0].checked == true) {
            type = 3;
        }

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnPayroll/Detail?handler=ProcessMandatory",
            data: {
                __RequestVerificationToken: token,
                "mandatoryType": type,
                "payrollId": $("#Id").val(),
                "employeeId": $("#ProcessMandatoryEmployeeId").val(),
                "isProcessInMonth": $("#IsProcessInMonth")[0].checked
            },
            success: function (data) {
                $("#processMandatoryDetailModal").modal("hide");

                GetPostMessage($("#PayrollNumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
            }
        });
    }
}

//Process Withholding
function CmdProcessWithholding() {
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnPayroll/Detail?handler=ProcessWithholding",
            data: {
                __RequestVerificationToken: token,
                "payrollId": $("#Id").val(),
                "employeeId": $("#ProcessWithholdingEmployeeId").val()
            },
            success: function (data) {
                $("#processWithholdingDetailModal").modal("hide");

                GetPostMessage($("#PayrollNumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
            }
        });
    }
}

//Process Totals
function CmdProcessTotals() {
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnPayroll/Detail?handler=ProcessTotals",
            data: {
                __RequestVerificationToken: token,
                "payrollId": $("#Id").val(),
                "employeeId": $("#ProcessTotalsEmployeeId").val()
            },
            success: function (data) {
                $("#processTotalsDetailModal").modal("hide");

                GetPostMessage($("#PayrollNumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
            }
        });
    }
}

//Payroll Input Events
function onDateChange(e) {
    $isDirty = true;
}

function onEntityComboboxChange(e)
{
    $isDirty = true;

    if (e.sender.element[0].id == "EditDepartmentId") {
        var comboBox = $("#EditEmployeeId").getKendoComboBox();

        if ($("#EditDepartmentId").val() != '') {
            comboBox.dataSource.filter({
                logic: "or",
                filters: [
                    { field: "DepartmentId", operator: "eq", value: parseInt($("#EditDepartmentId").val()) },
                ]
            });
        }
        else {
            comboBox.dataSource.filter({});
        }
    }


    if (e.sender.element[0].id == "ProcessDepartmentId")
    {
        var comboBox = $("#ProcessEmployeeId").getKendoComboBox();

        if ($("#ProcessDepartmentId").val() != '')
        {
            comboBox.dataSource.filter({
                logic: "or",
                filters: [
                    { field: "DepartmentId", operator: "eq", value: parseInt($("#ProcessDepartmentId").val()) },
                ]
            });
        }
        else
        {
            comboBox.dataSource.filter({});
        }
    }

    if (e.sender.element[0].id == "ComputeDepartmentId") {
        var comboBox = $("#ComputeEmployeeId").getKendoComboBox();

        if ($("#ComputeDepartmentId").val() != '') {
            comboBox.dataSource.filter({
                logic: "or",
                filters: [
                    { field: "DepartmentId", operator: "eq", value: parseInt($("#ComputeDepartmentId").val()) },
                ]
            });
        }
        else {
            comboBox.dataSource.filter({});
        }
    }
}

function onCheckboxChange(e)
{
    $isDirty = true;

    if (e.sender.element[0].id == "ProcessMandatorySSS")
    {
        $("#ProcessMandatoryPHIC")[0].checked = false;
        $("#ProcessMandatoryHDMF")[0].checked = false;
    }

    if (e.sender.element[0].id == "ProcessMandatoryPHIC")
    {
        $("#ProcessMandatorySSS")[0].checked = false;
        $("#ProcessMandatoryHDMF")[0].checked = false;
    }

    if (e.sender.element[0].id == "ProcessMandatoryHDMF")
    {
        $("#ProcessMandatorySSS")[0].checked = false;
        $("#ProcessMandatoryPHIC")[0].checked = false;
    }
}

function onIsApprovedCheckboxChange() {
    $isDirty = true;
}

function onSearchEmployee() {
    //The supported operators are:

    //"eq"(equal to)
    //"neq"(not equal to)
    //"isnull"(is equal to null)
    //"isnotnull"(is not equal to null)
    //"lt"(less than)
    //"lte"(less than or equal to)
    //"gt"(greater than)
    //"gte"(greater than or equal to)
    //"startswith"
    //"doesnotstartwith"
    //"endswith"
    //"doesnotendwith"
    //"contains"
    //"doesnotcontain"
    //"isempty"
    //"isnotempty"

    var trnDtrLines = $("#TrnPayrollLines").getKendoGrid();

    if (trnDtrLines != null) {
        trnDtrLines.dataSource.filter({ field: "EmployeeName", operator: "contains", value: $("#SearchEmployee").val() });
    }
}

function gotoEmployeeDetail(id) {
    window.open(window.location.origin + "/MstEmployee?handler=EmployeeDetail&id=" + id, "_blank").focus();

    $("#ApprovedBy").focus();
}

function onFKPayrollTypeChange()
{
    $isDirty = true;
}

function onFKTaxCodeChange()
{
    $isDirty = true;
}

//FK client template datasource 
function GetEmployeeText(data) {
    var result = "";

    if ($employees == null) {
        $.ajax({
            url: "/TrnPayroll/Detail?handler=Employees",
            type: "GET",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            async: false,
            data: { payrollGroupId: $("#PayrollGroupId").val() },
            success: function (data) {
                $employees = data;
            }
        });
    }

    for (let ctr = 0; ctr < $employees.length; ctr++) {
        if ($employees[ctr].Id == data.EmployeeId) {
            result = $employees[ctr].FullName;

            break;
        }
    }

    return result;
}

function EmployeeIdRouteValues(e) {
    return {
        payrollGroupId: $("#PayrollGroupId").val()
    };
}

function ExcelExport(e) {
    var sheet = e.workbook.sheets[0];
    for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
        var sheet = e.workbook.sheets[0];
        for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
            var row = sheet.rows[rowIndex];
            //row.cells[1].format = "[Blue]#,##0.0_);[Red](#,##0.0);0.0;"

            for (let ctr = 0; ctr < $employees.length; ctr++) {
                if ($employees[ctr].Id == row.cells[0].value) {
                    row.cells[0].value = $employees[ctr].FullName;

                    break;
                }
            }
        }
    }
}


//Methods
function turnPage() {
    $isTurnPage = true;
    $SelectedNavigationControl = "turnPage";

    if ($isDirty) {
        $("#confirmSaveDialog").data("kendoDialog").open();
        $("#showDialogBtn").fadeOut();
    }

    if (!$isDirty) {
        loadPartialViewViaTurnPage();
    }   
}

function loadPartialView(id)
{
    $("#loading").show();

    $("#detailFormView").empty();

    if (id == 0) {

        $("#detailFormView").load("/TrnPayroll/Detail?handler=Add",
            {
                __RequestVerificationToken: $("[name='__RequestVerificationToken']").val(),
                payrollGroupId: $("#payrollGroupCmb").val()
            },
            function (response, status, xhr) {
            if (status == "error") {
                GetErrorMessage("error", "Add");
            }
            else {
                $("#loading").hide();
            }
        });
    }
    else {
        $("#detailFormView").load("/TrnPayroll/Detail?Id=" + id, function (response, status, xhr) {
            if (status == "error") {
                GetErrorMessage("error", "Edit");
            }
            else {
                $("#loading").hide();
            }
        });
    }
}

function loadPartialViewViaTurnPage() {
    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnPayroll/Detail?handler=TurnPage",
        data: {
            __RequestVerificationToken: token,
            "id": $("#Id").val(),
            "payrollGroupId": $("#PayrollGroupId").val(),
            "action": $SelectedAction
        },
        success: function (data) {
            if (data.Id != 0) {
                loadPartialView(data.Id);
            }
        }
    });
}

function loadPartialViewProcessDtr() {
    $("#processDtrView").empty();
    $("#processDtrView").load("/TrnPayroll/ProcessDtr?handler=Load");
}

function loadPartialViewProcessOtherIncome() {
    $("#processOtherIncomeView").empty();
    $("#processOtherIncomeView").load("/TrnPayroll/ProcessOtherIncome?handler=Load");
}

function loadPartialViewProcessOtherDeduction() {
    $("#processOtherDeductionView").empty();
    $("#processOtherDeductionView").load("/TrnPayroll/ProcessOtherDeduction?handler=Load");
}

function loadPartialViewProcessMandatory() {
    $("#processMandatoryView").empty();
    $("#processMandatoryView").load("/TrnPayroll/ProcessMandatory?handler=Load");
}

function loadPartialViewProcessWithholding() {
    $("#processWithholdingView").empty();
    $("#processWithholdingView").load("/TrnPayroll/ProcessWithholding?handler=Load");
}

function loadPartialViewProcessTotals() {
    $("#processTotalsView").empty();
    $("#processTotalsView").load("/TrnPayroll/ProcessTotals?handler=Load");
}

function loadPartialViewExpandSalary(id) {
    $("#expandSalaryView").empty();
    $("#expandSalaryView").load("/TrnPayroll/ExpandSalary?id=" + id);
}

function loadPartialViewExpandPOI(id) {
    $("#expandPOIView").empty();
    $("#expandPOIView").load("/TrnPayroll/OtherIncome?id=" + id);
}

function loadPartialViewExpandPOIN(id) {
    $("#expandPOINView").empty();
    $("#expandPOINView").load("/TrnPayroll/OtherIncomeNonTax?id=" + id);
}

function loadPartialViewExpandPOD(id) {
    $("#expandPODView").empty();
    $("#expandPODView").load("/TrnPayroll/OtherDeduction?id=" + id);
}

