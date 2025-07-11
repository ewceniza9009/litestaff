//For Payroll Other Deduction
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
$otherDeduction = null;
$loan = null;

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
    resizeTrnGrids();
    applyMobileSizesToDetailGrid("TrnPayrollOtherDeductionLines")
}

function forgeryToken() {
    return {
        __RequestVerificationToken: kendo.antiForgeryTokens().__RequestVerificationToken,
        payrollGroupId: $("#payrollGroupCmb").val()
    };
}

function forgeryToken2() {
    return {
        __RequestVerificationToken: kendo.antiForgeryTokens().__RequestVerificationToken
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

function backToMainGrid()
{
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

function CmdImportOtherDeduction() {
    if ($("#Id").val() > 0) {
        var headers = {};

        headers["RequestVerificationToken"] = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        var fileInput = $('#file')[0];
        var file = fileInput.files[0];
        var formData = new FormData();
        formData.append('file', file);

        formData.append("Id", $("#Id").val());

        $.ajax({
            async: true,
            headers: headers,
            data: formData,
            processData: false,
            contentType: false,
            type: 'POST',
            url: "/TrnPayrollOtherDeduction/Detail?handler=ImportOtherDeduction",
            success: function (result) {
                $("#processDetailModal").modal("hide");

                GetPostMessage($("#Podnumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
            }
        });
    }
    else {
        GetWarningMessage("Please save the record first.");
    }
}

function CmdLoans() {
    loadPartialViewLoan();

    $("#loanDetailModal").modal("show");
}

function CmdQuickEncode()
{
    loadPartialViewEncode();

    $("#encodeDetailModal").modal("show");
}

function CmdPreview() {
    if ($("#Id").val() == 0) {
        GetWarningMessage("Please save the record first before printing.");
    }
    else {
        window.open(window.location.origin + "/TrnPayrollOtherDeduction/OtherDeductionReport?paramId=" + $("#Id").val(), '_blank').focus();
    }
}

function CmdDeleteModal(e) {
    $("#confirmDeleteDialog").data("kendoDialog").open();
}

function CmdDelete()
{
    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnPayrollOtherDeduction/Detail?handler=Delete",
        data:
        {
            __RequestVerificationToken: token,
            "Id": $("#Id").val()
        },
        success: function (data) {
            GetWarningMessage("Record [" + $("#Podnumber").val() + "] is successfully deleted.");

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
    frmData.Podnumber = $("#Podnumber").val();
    frmData.PayrollGroupId = $("#PayrollGroupId").val();
    frmData.Remarks = $("#RemarksHeader").val();
    frmData.IsLocked = $("#IsLocked")[0].checked;

    if (frmData.IsLocked == false) {
        var response = confirm("Do you want to lock(YES) the record or just leave(Cancel) it for later?.");

        if (response) {
            frmData.IsLocked = true;
        }
    }

    var frmDataSF1 = { TrnPayrollOtherDeductionLines: GetGridViewJsonData($("#TrnPayrollOtherDeductionLines")) };

    var dataValue = { ...frmData, ...frmDataSF1 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnPayrollOtherDeduction/Detail?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "pod": dataValue
        },
        success: function (data) {
            GetSaveMessage($("#Podnumber").val());

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

//Edit POD Application
function CmdPayrollOtherDeductionLoans() {
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnPayrollOtherDeduction/Detail?handler=Loans",
            data: {
                __RequestVerificationToken: token,
                "podId": $("#Id").val(),
                "payrollGroupId": $("#PayrollGroupId").val(),
                "loanNumber": $("#LoanNumber").val(),
                "dateFilter": $("#DateFilter").val(),
                "employeeIdFilter": $("#EmployeeIdFilter").val()
            },
            success: function (data) {
                $("#loanDetailModal").modal("hide");

                GetPostMessage($("#Podnumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                GetWarningMessage("Error occured, Server is unresponsive.");
            }
        });
    }
    else {
        GetWarningMessage("Please save the record first.");
    }
}

function CmdPayrollOtherDeductionQuickEncode()
{
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnPayrollOtherDeduction/Detail?handler=QuickEncode",
            data: {
                __RequestVerificationToken: token,
                "podId": $("#Id").val(),
                "payrollGroupId": $("#PayrollGroupId").val(),
                "employeeId": $("#EncodeEmployeeId").val(),
                "tmpOtherDeductions": GetGridViewJsonData($("#gridOtherDeduction"))
            },
            success: function (data) {
                $("#encodeDetailModal").modal("hide");

                GetPostMessage($("#Podnumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                GetWarningMessage("Error occured, Server is unresponsive.");
            }
        });
    }
    else
    {
        GetWarningMessage("Please save the record first.");
    }
}

//POD Application Line Grid
function CmdAddPayrollOtherDeductionLine(e)
{
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/TrnPayrollOtherDeduction/Detail?handler=AddPayrollOtherDeductionLine",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            PODId: $("#Id").val()
        },
        success: function (data) {
            data.Date = new Date(data.Date).toLocaleDateString();
            data.EmployeeId = null;

            $("#TrnPayrollOtherDeductionLines").getKendoGrid()
                .dataSource.insert(0, data);
        }
    });
}

function CmdDeleteAllLines(e) {
    $isDirty = true;

    var subGrid = $("#TrnPayrollOtherDeductionLines").getKendoGrid();
    var subGridData = $("#TrnPayrollOtherDeductionLines").getKendoGrid().dataSource.data();

    for (let i = 0; i < subGridData.length; i++) {
        subGridData[i].set("IsDeleted", true);
    }

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function CmdDeletePayrollOtherDeductionLine(e)
{
    $isDirty = true;

    var subGrid = $("#TrnPayrollOtherDeductionLines").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Payroll Other Deduction Input Events
function onDateChange(e) {
    $isDirty = true;
}

function onEntityComboboxChange(e)
{
    $isDirty = true;
}

function onCheckboxChange()
{
    $isDirty = true;
}

//FK client template datasource 
function GetEmployeeText(data) {
    var result = "";

    if ($employees == null) {
        $.ajax({
            url: "/TrnPayrollOtherDeduction/Detail?handler=Employees2",
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

function GetDeductionText(data) {
    var result = "";

    if ($otherDeduction == null) {
        $.ajax({
            url: "/TrnPayrollOtherDeduction/Detail?handler=Deductions",
            type: "GET",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            async: false,
            success: function (data) {
                $otherDeduction = data;
            }
        });
    }

    for (let ctr = 0; ctr < $otherDeduction.length; ctr++) {
        if ($otherDeduction[ctr].Id == data.OtherDeductionId) {
            result = $otherDeduction[ctr].OtherDeduction;

            break;
        }
    }

    return result;
}

function GetLoanText(data) {
    var result = "";

    //$.ajax({
    //    url: "/TrnPayrollOtherDeduction/Detail?handler=LoanText",
    //    type: "GET",
    //    dataType: 'json',
    //    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
    //    async: false,
    //    data: { "loanId": data.EmployeeLoanId },
    //    success: function (data) {
    //        result = data;
    //    }
    //});

    if ($loan == null) {
        $.ajax({
            url: "/TrnPayrollOtherDeduction/Detail?handler=Loans2",
            type: "GET",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            async: false,
            success: function (data) {
                $loan = data;
            }
        });
    }

    for (let ctr = 0; ctr < $loan.length; ctr++) {
        if ($loan[ctr].Id == data.LoanId) {
            result = $loan[ctr].LoanDate2;

            break;
        }
    }

    return result;
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

            for (let ctr = 0; ctr < $otherDeduction.length; ctr++) {
                if ($otherDeduction[ctr].Id == row.cells[1].value) {
                    row.cells[1].value = $otherDeduction[ctr].OtherDeduction;

                    break;
                }
            }

            $.ajax({
                url: "/TrnPayrollOtherDeduction/Detail?handler=LoanText",
                type: "GET",
                dataType: 'json',
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                async: false,
                data: { "loanId": row.cells[2].value },
                success: function (data) {
                    row.cells[2].value = data;
                }
            });            
        }
    }
} 

function LoanIdRouteValues(e) {
    var row = $("#TrnPayrollOtherDeductionLines").find(".k-edit-cell").parent();
    var grid = $("#TrnPayrollOtherDeductionLines").data("kendoGrid");

    var item = grid.dataItem(row);

    if (item != null) {
        return {
            employeeId: item.EmployeeId
        };
    }

    return {};
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

        $("#detailFormView").load("/TrnPayrollOtherDeduction/Detail?handler=Add",
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
        $("#detailFormView").load("/TrnPayrollOtherDeduction/Detail?Id=" + id, function (response, status, xhr) {
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
        url: "/TrnPayrollOtherDeduction/Detail?handler=TurnPage",
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

function loadPartialViewLoan() {
    $("#loanView").empty();
    $("#loanView").load("/TrnPayrollOtherDeduction/Loans?handler=Load", function (response, status, xhr) {

    });
}

function loadPartialViewEncode() {
    $("#encodeView").empty();
    $("#encodeView").load("/TrnPayrollOtherDeduction/QuickEncode?handler=Load", function (response, status, xhr) {

    });
}
