﻿//For Leave Application
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
$leaves = null;

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
    applyMobileSizesToDetailGrid("TrnLeaveApplicationLines")
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

function CmdImportLeave() {
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
            url: "/TrnLeaveApplication/Detail?handler=ImportLeave",
            success: function (result) {
                $("#processDetailModal").modal("hide");

                GetPostMessage($("#Lanumber").val());

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
        window.open(window.location.origin + "/TrnLeaveApplication/LAReport?paramId=" + $("#Id").val(), '_blank').focus();
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
        url: "/TrnLeaveApplication/Detail?handler=Delete",
        data:
        {
            __RequestVerificationToken: token,
            "Id": $("#Id").val()
        },
        success: function (data) {
            GetWarningMessage("Record [" + $("#Lanumber").val() + "] is successfully deleted.");

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
    frmData.Lanumber = $("#Lanumber").val();
    frmData.PayrollGroupId = $("#PayrollGroupId").val();
    frmData.Remarks = $("#RemarksHeader").val();
    frmData.IsLocked = $("#IsLocked")[0].checked;

    if (frmData.IsLocked == false) {
        var response = confirm("Do you want to lock(YES) the record or just leave(Cancel) it for later?.");

        if (response) {
            frmData.IsLocked = true;
        }
    }

    var frmDataSF1 = { TrnLeaveApplicationlines: GetGridViewJsonData($("#TrnLeaveApplicationLines")) };

    for (var item of frmDataSF1.TrnLeaveApplicationlines) {
        if (item.Date != null) {
            item.Date = new Date(item.Date).toLocaleDateString();
        }
    }

    var dataValue = { ...frmData, ...frmDataSF1 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnLeaveApplication/Detail?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "leaveApp": dataValue
        },
        success: function (data) {
            GetSaveMessage($("#Lanumber").val());

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

//Edit Leave Application
function CmdLeaveApplicationQuickEncode()
{
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnLeaveApplication/Detail?handler=QuickEncode",
            data: {
                __RequestVerificationToken: token,
                "laId": $("#Id").val(),
                "payrollGroupId": $("#PayrollGroupId").val(),
                "dateStart": $("#EncodeDateStart").val(),
                "dateEnd": $("#EncodeDateEnd").val(),
                "employeeId": $("#EncodeEmployeeId").val(),
                "leaveId": $("#EncodeLeaveId").val(),
                "numberOfHours": $("#EncodeNoOfHours").val(), 
                "withPay": $("#EncodeWithPay")[0].checked, 
                "debitToLedger": $("#EncodeDebit")[0].checked, 
                "remarks": $("#EncodeRemarks").val()
            },
            success: function (data) {
                $("#encodeDetailModal").modal("hide");

                GetPostMessage($("#Lanumber").val());

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

//Leave Application Line Grid
function CmdAddLeaveApplicationLine(e)
{
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/TrnLeaveApplication/Detail?handler=AddLeaveApplicationLine",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            LAId: $("#Id").val()
        },
        success: function (data) {
            data.Date = new Date(data.Date).toLocaleDateString();
            data.EmployeeId = null;

            $("#TrnLeaveApplicationLines").getKendoGrid()
                .dataSource.insert(data);
        }
    });
}

function CmdDeleteAllLines(e) {
    $isDirty = true;

    var subGrid = $("#TrnLeaveApplicationLines").getKendoGrid();
    var subGridData = $("#TrnLeaveApplicationLines").getKendoGrid().dataSource.data();

    for (let i = 0; i < subGridData.length; i++) {
        subGridData[i].set("IsDeleted", true);
    }

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function CmdDeleteLeaveApplicationLine(e)
{
    $isDirty = true;

    var subGrid = $("#TrnLeaveApplicationLines").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Leave Application Input Events
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
            url: "/TrnLeaveApplication/Detail?handler=Employees",
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

function GetLeaveText(data) {
    var result = "";

    if ($leaves == null) {
        $.ajax({
            url: "/TrnLeaveApplication/Detail?handler=Leaves",
            type: "GET",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            async: false,
            success: function (data) {
                $leaves = data;
            }
        });
    }

    for (let ctr = 0; ctr < $leaves.length; ctr++) {
        if ($leaves[ctr].Id == data.LeaveId) {
            result = $leaves[ctr].Leave;

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

            for (let ctr = 0; ctr < $leaves.length; ctr++) {
                if ($leaves[ctr].Id == row.cells[1].value) {
                    row.cells[1].value = $leaves[ctr].Leave;;

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

        $("#detailFormView").load("/TrnLeaveApplication/Detail?handler=Add",
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
        $("#detailFormView").load("/TrnLeaveApplication/Detail?Id=" + id, function (response, status, xhr) {
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
        url: "/TrnLeaveApplication/Detail?handler=TurnPage",
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

function loadPartialViewEncode() {
    $("#encodeView").empty();
    $("#encodeView").load("/TrnLeaveApplication/QuickEncode?handler=Load", function (response, status, xhr) {

    });
}
