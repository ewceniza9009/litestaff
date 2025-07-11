//For Payroll Other Income
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
$otherIncome = null;

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
    applyMobileSizesToDetailGrid("TrnPayrollOtherIncomeLines")
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

function CmdImportOtherIncome() {
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
            url: "/TrnPayrollOtherIncome/Detail?handler=ImportOtherIncome",
            success: function (result) {
                $("#processDetailModal").modal("hide");

                GetPostMessage($("#Poinumber").val());

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

function Cmd13Month() {
    loadPartialView13Month();

    $("#oi13MonthDetailModal").modal("show");
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
        window.open(window.location.origin + "/TrnPayrollOtherIncome/OtherIncomeReport?paramId=" + $("#Id").val(), '_blank').focus();
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
        url: "/TrnPayrollOtherIncome/Detail?handler=Delete",
        data:
        {
            __RequestVerificationToken: token,
            "Id": $("#Id").val()
        },
        success: function (data) {
            GetWarningMessage("Record [" + $("#Poinumber").val() + "] is successfully deleted.");

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
    frmData.Poinumber = $("#Poinumber").val();
    frmData.PayrollGroupId = $("#PayrollGroupId").val();
    frmData.Remarks = $("#RemarksHeader").val();
    frmData.IsLocked = $("#IsLocked")[0].checked;

    if (frmData.IsLocked == false) {
        var response = confirm("Do you want to lock(YES) the record or just leave(Cancel) it for later?.");

        if (response) {
            frmData.IsLocked = true;
        }
    }

    var frmDataSF1 = { TrnPayrollOtherIncomeLines: GetGridViewJsonData($("#TrnPayrollOtherIncomeLines")) };

    var dataValue = { ...frmData, ...frmDataSF1 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnPayrollOtherIncome/Detail?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "poi": dataValue
        },
        success: function (data) {
            GetSaveMessage($("#Poinumber").val());

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

//Edit POI Application
function CmdPayrollOtherIncomeGenerate13Month()
{
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnPayrollOtherIncome/Detail?handler=OI13Month",
            data: {
                __RequestVerificationToken: token,
                "poiId": $("#Id").val(),
                "payrollGroupId": $("#PayrollGroupId").val(),
                "otherIncomeId": $("#OI13OtherIncomeId").val(),
                "employeeId": $("#OI13EmployeeId").val(),
                "startPayNo": $("#OI13StartPayNo").val(),
                "endPayNo": $("#OI13EndPayNo").val(),
                "noOfPayroll": $("#NoOfPayroll").val(),
            },
            success: function (data) {
                $("#oi13MonthDetailModal").modal("hide");

                GetPostMessage($("#Poinumber").val());

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

function CmdPayrollOtherIncomeQuickEncode()
{
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnPayrollOtherIncome/Detail?handler=QuickEncode",
            data: {
                __RequestVerificationToken: token,
                "poiId": $("#Id").val(),
                "payrollGroupId": $("#PayrollGroupId").val(),
                "employeeId": $("#EncodeEmployeeId").val(),
                "tmpOtherIncomes": GetGridViewJsonData($("#gridOtherIncome"))
            },
            success: function (data) {
                $("#encodeDetailModal").modal("hide");

                GetPostMessage($("#Poinumber").val());

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

//POI Application Line Grid
function CmdAddPayrollOtherIncomeLine(e)
{
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/TrnPayrollOtherIncome/Detail?handler=AddPayrollOtherIncomeLine",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            POIId: $("#Id").val()
        },
        success: function (data) {
            data.Date = new Date(data.Date).toLocaleDateString();
            data.EmployeeId = null;

            $("#TrnPayrollOtherIncomeLines").getKendoGrid()
                .dataSource.insert(data);
        }
    });
}

function CmdDeleteAllLines(e) {
    $isDirty = true;

    var subGrid = $("#TrnPayrollOtherIncomeLines").getKendoGrid();
    var subGridData = $("#TrnPayrollOtherIncomeLines").getKendoGrid().dataSource.data();

    for (let i = 0; i < subGridData.length; i++) {
        subGridData[i].set("IsDeleted", true);
    }

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function CmdDeletePayrollOtherIncomeLine(e)
{
    $isDirty = true;

    var subGrid = $("#TrnPayrollOtherIncomeLines").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Payroll Other Income Input Events
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
            url: "/TrnPayrollOtherIncome/Detail?handler=Employees",
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


function GetIncomeText(data) {
    var result = "";

    if ($otherIncome == null) {
        $.ajax({
            url: "/TrnPayrollOtherIncome/Detail?handler=Incomes",
            type: "GET",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            async: false,
            success: function (data) {
                $otherIncome = data;
            }
        });
    }

    for (let ctr = 0; ctr < $otherIncome.length; ctr++) {
        if ($otherIncome[ctr].Id == data.OtherIncomeId) {
            result = $otherIncome[ctr].OtherIncome;

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

            for (let ctr = 0; ctr < $otherIncome.length; ctr++) {
                if ($otherIncome[ctr].Id == row.cells[1].value) {
                    row.cells[1].value = $otherIncome[ctr].OtherIncome;

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

        $("#detailFormView").load("/TrnPayrollOtherIncome/Detail?handler=Add",
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
        $("#detailFormView").load("/TrnPayrollOtherIncome/Detail?Id=" + id, function (response, status, xhr) {
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
        url: "/TrnPayrollOtherIncome/Detail?handler=TurnPage",
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

function loadPartialView13Month()
{
    $("#oi13MonthView").empty();
    $("#oi13MonthView").load("/TrnPayrollOtherIncome/OtherIncome13Month?handler=Load", function (response, status, xhr) {

    });
}

function loadPartialViewEncode() {
    $("#encodeView").empty();
    $("#encodeView").load("/TrnPayrollOtherIncome/QuickEncode?handler=Load", function (response, status, xhr) {

    });
}
