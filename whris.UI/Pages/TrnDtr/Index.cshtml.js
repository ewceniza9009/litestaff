//For Dtr
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
$shiftCodes = null;
$dayTypes = null;

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

function onDetailGridDataBound()
{
    applyMobileSizesToDetailGrid("TrnDtrlines")

    $("#TrnDtrlinesMobile").hide();

    if (detectMob())
    {
        $("#TrnDtrlines").hide();
        $("#TrnDtrlinesMobile").show();
    }
}

function forgeryToken() {
    return {
        __RequestVerificationToken: kendo.antiForgeryTokens().__RequestVerificationToken,
        payrollGroupId: $("#payrollGroupCmb").val()
    };
}

function forgeryToken2() {
    return {
        __RequestVerificationToken: kendo.antiForgeryTokens().__RequestVerificationToken,
        startDate: $("#LogDateStart").val(),
        endDate: $("#LogDateEnd").val(),
        employeeId: $("#LogEmployeeId").val()
    };
}

//Button Commands and Lookup
function onPayrollGroupChange() {
    backToMainGrid();

    $.ajax({
        async: true,
        type: "GET",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnDtr/Detail?handler=PayrollGroupChange&payrollGroupId=" + $("#payrollGroupCmb").val(),
        success: function (data) {
            grid.dataSource.read();
        },
        error: function (error) {
            //Do nothing...
        }
    });
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

function onCellEdited(e)
{
    e.model.IsEdited = true;
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
        url: "/TrnDtr/Detail?handler=QuickChangeShift",
        data: {
            __RequestVerificationToken: token,
            "dtrId": $("#Id").val()
        },
        success: function (data) {
            $("#editDetailModal").modal("hide");

            GetPostMessage($("#Dtrnumber").val());

            loadPartialView($("#Id").val())
        },
        error: function (xhr, status, error) {
            GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
        }
    });
}

//Show Logs
function CmdShowLogs() {
    loadPartialViewLogs();

    $("#logsModal").modal("show");
}

function CmdExportLogsToExcellFull()
{
    var token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Get form values
    const employeeId = document.getElementById("LogEmployeeId").value;
    const dateStart = document.getElementById("LogDateStart").value;
    const dateEnd = document.getElementById("LogDateEnd").value;

    // Prepare form data
    const formData = new URLSearchParams();
    formData.append("__RequestVerificationToken", token);
    formData.append("employeeId", employeeId);
    formData.append("dateStart", dateStart);
    formData.append("dateEnd", dateEnd);

    // Fetch call
    fetch('/TrnDtr/Logs?handler=ExportLogsToExcelFull', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString()
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = 'Logs.xlsx';
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            a.remove();
        })
        .catch(error => {
            console.error('Fetch error:', error);
            GetWarningMessage("Error occurred, It's either server is unresponsive or the dates are invalid.");
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

function CmdMissingPunches()
{
    if ($("#Id").val() > 0) {
        loadPartialViewMissing();

        $("#missingPunchesModal").modal("show");
    }
    else
    {
        GetWarningMessage("Please save the dtr record first.");
    }   
}

function CmdQuickEdit()
{
    loadPartialViewEdit();

    $("#editDetailModal").modal("show");
}

function CmdQuickShift()
{
    $("#confirmQuickChangeDialog").data("kendoDialog").open();
}

function CmdCompute()
{
    loadPartialViewCompute();

    $("#computeDetailModal").modal("show");
}

function CmdProcess()
{
    loadPartialViewProcess();

    $("#processDetailModal").modal("show");
}

function CmdPreview()
{
    if ($("#Id").val() == 0) {
        GetWarningMessage("Please save the dtr record first before printing.");
    }
    else {
        var withRemarks = $("#WithRemarks")[0].checked;
        var inActive = $("#InActive")[0].checked;
        var employeeFilter = $("#SearchEmployee").val();

        window.open(window.location.origin + "/TrnDtr/DtrReport?paramId=" + $("#Id").val() + "&employeeFilter=" + employeeFilter + "&withRemarks=" + withRemarks + "&inActive=" + inActive, '_blank').focus();
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
        url: "/TrnDtr/Detail?handler=Delete",
        data:
        {
            __RequestVerificationToken: token,
            "Id": $("#Id").val()
        },
        success: function (data) {
            GetWarningMessage("Record [" + $("#Dtrnumber").val() + "] is successfully deleted.");

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

function CmdDeleteLogById(e) {
    var gridLogs = $("#gridLogs").getKendoGrid();
    var item = gridLogs.dataItem($(e.target).closest("tr"));

    var token = $('input[name="__RequestVerificationToken"]', $("#frmLogs")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnDtr/Logs?handler=Delete",
        data:
        {
            __RequestVerificationToken: token,
            "Id": item.Id
        },
        success: function (data) {
            GetWarningMessage("Record is successfully deleted.");

            gridLogs.dataSource.read();
        },
        error: function (error) {
            GetErrorMessage(error, "Delete");
        }
    });
}

function CmdSave()
{
    $("#btnSaveSpin").removeAttr("hidden")

    $("#btnSaveLock").hide();
    $("#btnSaveSpin").show();

    var frmData = GetFormData($("#frmDetail"));

    frmData.PeriodId = $("#PeriodId").val();
    frmData.Dtrnumber = $("#Dtrnumber").val();
    frmData.PayrollGroupId = $("#PayrollGroupId").val();

    frmData.IsLocked = $("#IsLocked")[0].checked;
    frmData.IsApproved = $("#IsApproved")[0].checked;
    frmData.IsComputeRestDay = $("#IsComputeRestDay")[0].checked;

    if (frmData.IsLocked == false )
    {
        var response = confirm("Do you want to lock(YES) the record or just leave(Cancel) it for later?.");

        if (response)
        {
            frmData.IsLocked = true;
        }
    }

    var frmDataSF1 = { TrnDtrlines: GetGridViewJsonData($("#TrnDtrlines")) };

    for (var item of frmDataSF1.TrnDtrlines)
    {
        if (item.Date != null)
        {
            item.Date = new Date(item.Date).toLocaleDateString();
        }

        if (item.TimeIn1 != null)
        {
            item.TimeIn1 = new Date(item.TimeIn1).toLocaleString();

            //var timePart = new Date(item.TimeIn1).toLocaleTimeString();
            //var datePart = new Date(item.Date).toLocaleDateString();            

            //item.TimeIn1 = datePart + ", " + timePart;            
        }

        if (item.TimeOut1 != null)
        {
            item.TimeOut1 = new Date(item.TimeOut1).toLocaleString();

            //var timePart = new Date(item.TimeOut1).toLocaleTimeString();
            //var datePart = new Date(item.Date).toLocaleDateString();

            //if (new Date(item.TimeOut1).getTime() < new Date(item.TimeIn1).getTime())
            //{
            //    datePart = addDays(new Date(item.Date), 1).toLocaleDateString()
            //}

            //item.TimeOut1 = datePart + ", " + timePart; 
        }

        if (item.TimeIn2 != null)
        {
            item.TimeIn2 = new Date(item.TimeIn2).toLocaleString();

            //var timePart = new Date(item.TimeIn2).toLocaleTimeString();
            //var datePart = new Date(item.Date).toLocaleDateString();

            //if (new Date(item.TimeIn2).getTime() < new Date(item.TimeIn1).getTime()) {
            //    datePart = addDays(new Date(item.Date), 1).toLocaleDateString()
            //}

            //item.TimeIn2 = datePart + ", " + timePart; 
        }

        if (item.TimeOut2 != null)
        {
            item.TimeOut2 = new Date(item.TimeOut2).toLocaleString();

            //var timePart = new Date(item.TimeOut2).toLocaleTimeString();
            //var datePart = new Date(item.Date).toLocaleDateString();

            //if (new Date(item.TimeOut2).getTime() < new Date(item.TimeIn1).getTime()) {
            //    datePart = addDays(new Date(item.Date), 1).toLocaleDateString()
            //}

            //item.TimeOut2 = datePart + ", " + timePart; 
        }
    }

    var dataValue = { ...frmData, ...frmDataSF1 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnDtr/Detail?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "dtr": dataValue
        },
        success: function (data) {
            GetSaveMessage($("#Dtrnumber").val());

            $("#btnSaveLock").show();
            $("#btnSaveSpin").hide();

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

            $("#btnSaveLock").show();
            $("#btnSaveSpin").hide();

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

function CmdSearchLogs()
{
    $("#gridLogs").data("kendoGrid").dataSource.read();
}

//Save Missing Punches
function CmdDtrMissingPunches()
{
    if ($("#Id").val() > 0) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        var datas = GetGridViewJsonData($("#gridMissingPunches"));

        for (var item of datas) {
            if (item.Date != null)
            {
                item.Date = new Date(item.Date).toLocaleDateString();
            }

            if (item.TimeIn1 != null) {
                item.TimeIn1 = new Date(item.TimeIn1).toLocaleString();
            }

            if (item.TimeOut1 != null) {
                item.TimeOut1 = new Date(item.TimeOut1).toLocaleString();
            }

            if (item.TimeIn2 != null) {
                item.TimeIn2 = new Date(item.TimeIn2).toLocaleString();
            }

            if (item.TimeOut2 != null) {
                item.TimeOut2 = new Date(item.TimeOut2).toLocaleString();
            }
        }

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnDtr/Detail?handler=UpdateMissingPunches",
            data: {
                __RequestVerificationToken: token,
                "records": datas
            },
            success: function (data) {
                $("#missingPunchesModal").modal("hide");

                GetPostMessage($("#Dtrnumber").val());

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

//Edit Dtr
function CmdDtrQuickEdit()
{
    if ($("#Id").val() > 0)
    {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/TrnDtr/Detail?handler=QuickEdit",
            data: {
                __RequestVerificationToken: token,
                "dtrId": $("#Id").val(),
                "dateStart": $("#EditDateStart").val(),
                "dateEnd": $("#EditDateEnd").val(),
                "departmentId": $("#EditDepartmentId").val(),
                "employeeId": $("#EditEmployeeId").val(),
                "timeIn1": $("#TimeIn1").val(),
                "timeOut1": $("#TimeOut1").val(),
                "timeIn2": $("#TimeIn2").val(),
                "timeOut2": $("#TimeOut2").val()
            },
            success: function (data) {
                $("#editDetailModal").modal("hide");

                GetPostMessage($("#Dtrnumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
            }
        });
    }
}

//Process Dtr
function CmdDtrProcess()
{
    if ($("#Id").val() > 0) {
        $("#btnProcessSpin").removeAttr("hidden")

        $("#btnProcessGear").hide();
        $("#btnProcessSpin").show();

        var headers = {};

        headers["RequestVerificationToken"] = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        var fileInput = $('#file')[0];
        var file = fileInput.files[0];
        var formData = new FormData();
        formData.append('file', file);

        formData.append("DTRId", $("#Id").val());
        formData.append("PeriodId", $("#PeriodId").val());
        formData.append("Dtrnumber", $("#Dtrnumber").val());
        formData.append("Dtrdate", $("#Dtrdate").val());
        formData.append("PayrollGroupId", $("#PayrollGroupId").val());
        formData.append("Remarks", $("#Remarks").val());

        formData.append("OvertTimeId", $("#OvertTimeId").val());
        formData.append("LeaveApplicationId", $("#LeaveApplicationId").val());
        formData.append("ChangeShiftId", $("#ChangeShiftId").val());
        formData.append("PreparedBy", $("#PreparedBy").val());
        formData.append("CheckedBy", $("#CheckedBy").val());
        formData.append("ApprovedBy", $("#ApprovedBy").val());

        formData.append("DepartmentId", $("#ProcessDepartmentId").val());
        formData.append("EmployeeId", $("#ProcessEmployeeId").val());
        formData.append("StartDate", $("#ProcessDateStart").val());
        formData.append("EndDate", $("#ProcessDateEnd").val());

        $.ajax({
            async: true,
            headers: headers,
            data: formData,
            processData: false,
            contentType: false,
            type: 'POST',
            url: "/TrnDtr/Detail?handler=DtrProcess",
            success: function (result) {
                $("#processDetailModal").modal("hide");

                $("#btnProcessGear").show();
                $("#btnProcessSpin").hide();

                GetPostMessage($("#Dtrnumber").val());

                loadPartialView($("#Id").val())
            },
            error: function (xhr, status, error) {
                $("#btnProcessGear").show();
                $("#btnProcessSpin").hide();

                GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
            }
        });
    }
    else
    {
        GetWarningMessage("Please save the record first.");
    }
}

//Compute Dtr
function CmdDtrCompute()
{
    $("#btnComputeSpin").removeAttr("hidden")

    $("#btnComputeGear").hide();
    $("#btnComputeSpin").show();

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnDtr/Detail?handler=DtrCompute",
        data: {
            __RequestVerificationToken: token,
            "dtrId": $("#Id").val(),
            "empId": $("#ComputeEmployeeId").val(),
            "startDate": $("#DateStart").val(),
            "endDate": $("#DateEnd").val()
        },
        success: function (data) {
            $("#computeDetailModal").modal("hide");

            $("#btnComputeGear").show();
            $("#btnComputeSpin").hide();

            GetPostMessage($("#Dtrnumber").val());

            loadPartialView($("#Id").val())
        },
        error: function (xhr, status, error) {
            $("#btnComputeGear").show();
            $("#btnComputeSpin").hide();

            GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
        }
    });
}

//Dtr Line Grid
function CmdAddDtrLine(e)
{
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/TrnDtr/Detail?handler=AddDtrLine",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            dtrId: $("#Id").val()
        },
        success: function (data) {
            data.Date = new Date(data.Date).toLocaleDateString();
            data.EmployeeId = null;

            $("#TrnDtrlines").getKendoGrid()
                .dataSource.insert(data);
        }
    });
}

function CmdDeleteAllLines(e)
{
    $isDirty = true;

    var subGrid = $("#TrnDtrlines").getKendoGrid();
    var subGridData = $("#TrnDtrlines").getKendoGrid().dataSource.data();

    for (let i = 0; i < subGridData.length; i++) {
        subGridData[i].set("IsDeleted", true);
    }

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function CmdDeleteDtrLine(e)
{
    $isDirty = true;

    var subGrid = $("#TrnDtrlines").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Dtr Input Events
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

function onCheckboxChange()
{
    $isDirty = true;
}

function onIsApprovedCheckboxChange()
{
    $isDirty = true;
}

function onSearchEmployee()
{
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

    var trnDtrLines = $("#TrnDtrlines").getKendoGrid();

    if (trnDtrLines != null)
    {
        trnDtrLines.dataSource.filter({ field: "EmployeeName", operator: "contains", value: $("#SearchEmployee").val() });
    }
}

function onSearchEmployeeOnMissingPunches() {

    var gridMissingPunches = $("#gridMissingPunches").getKendoGrid();

    if (gridMissingPunches != null) {
        gridMissingPunches.dataSource.filter({ field: "Employee", operator: "contains", value: $("#SearchEmployeeOnMissingPunches").val() });
    }
}

function gotoEmployeeDetail(id)
{
    window.open(window.location.origin + "/MstEmployee?handler=EmployeeDetail&id=" + id, "_blank").focus();

    $("#SearchEmployee").focus();
}

function gotoShiftCodeDetail(id)
{
    window.open(window.location.origin + "/MstShiftCode?handler=ShiftCodeDetail&id=" + id, "_blank").focus();

    $("#SearchEmployee").focus();
}

//Missing Punches Events
function gridTimeIn1CellChanged(e) {
    var textInput = $(e).children()[0].children[0].value;
    $(e).children()[0].children[0].value = convertToTimeFormat(textInput);
}

function gridTimeOut1CellChanged(e) {
    var textInput = $(e).children()[0].children[0].value;
    $(e).children()[0].children[0].value = convertToTimeFormat(textInput);
}

function gridTimeIn2CellChanged(e) {
    var textInput = $(e).children()[0].children[0].value;
    $(e).children()[0].children[0].value = convertToTimeFormat(textInput);
}

function gridTimeOut2CellChanged(e) {
    var textInput = $(e).children()[0].children[0].value;
    $(e).children()[0].children[0].value = convertToTimeFormat(textInput);
}

//FK client template datasource 
function GetEmployeeText(data) {
    var result = "";

    if ($employees == null) {
        $.ajax({
            url: "/TrnDtr/Detail?handler=Employees",
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

function GetShiftCodeText(data) {
    var result = "";

    if ($shiftCodes == null) {
        $.ajax({
            url: "/TrnDtr/Detail?handler=ShiftCodes",
            type: "GET",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            async: false,
            success: function (data) {
                $shiftCodes = data;
            }
        });
    }

    for (let ctr = 0; ctr < $shiftCodes.length; ctr++) {
        if ($shiftCodes[ctr].Id == data.ShiftCodeId) {
            result = $shiftCodes[ctr].ShiftCode;

            break;
        }
    }

    return result;
}

function GetDayTypeText(data) {
    var result = "";

    if ($dayTypes == null) {
        $.ajax({
            url: "/TrnDtr/Detail?handler=DayTypes",
            type: "GET",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            async: false,
            success: function (data) {
                $dayTypes = data;
            }
        });
    }

    for (let ctr = 0; ctr < $dayTypes.length; ctr++) {
        if ($dayTypes[ctr].Id == data.DayTypeId) {
            result = $dayTypes[ctr].DayType;

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

            for (let ctr = 0; ctr < $shiftCodes.length; ctr++) {
                if ($shiftCodes[ctr].Id == row.cells[1].value) {
                    row.cells[1].value = $shiftCodes[ctr].ShiftCode;

                    break;
                }
            }

            for (let ctr = 0; ctr < $dayTypes.length; ctr++) {
                if ($dayTypes[ctr].Id == row.cells[3].value) {
                    row.cells[3].value = $dayTypes[ctr].DayType;

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

        $("#detailFormView").load("/TrnDtr/Detail?handler=Add",
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
                $("#InActive")[0].disabled = true;
                WithRemarksSetEvent();
            }
        });
    }
    else {
        $("#detailFormView").load("/TrnDtr/Detail?Id=" + id, function (response, status, xhr) {
            if (status == "error") {
                GetErrorMessage("error", "Edit");
            }
            else {
                $("#loading").hide();
                $("#InActive")[0].disabled = true;
                WithRemarksSetEvent();

                var grid = $("#TrnDtrlines").data("kendoGrid");
                grid.bind("edit", onGridEdit);
            }
        });
    }
}

function WithRemarksSetEvent()
{
    // Initialize the Kendo UI CheckBox
    var checkbox = $("#WithRemarks").kendoCheckBox().data("kendoCheckBox");

    // Bind the change event to the CheckBox
    checkbox.bind("change", function (e) {
        if ($("#WithRemarks")[0].checked)
        {
            $("#InActive")[0].disabled = false;
        }
        else
        {
            $("#InActive")[0].disabled = true;
            $("#InActive")[0].checked = false;
        }

    });
}

function loadPartialViewViaTurnPage() {
    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/TrnDtr/Detail?handler=TurnPage",
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

function loadPartialViewLogs() {
    $("#logsView").empty();
    $("#logsView").load("/TrnDtr/Logs?handler=Load", function (response, status, xhr) {});
}

function loadPartialViewMissing() {
    $("#missingView").empty();
    $("#missingView").load("/TrnDtr/MissingPunches?dtrId=" + $("#Id").val(), function (response, status, xhr) {

    });
}

function loadPartialViewEdit() {
    $("#editView").empty();
    $("#editView").load("/TrnDtr/QuickEdit?handler=Load", function (response, status, xhr) {
        $("#EditDateStart").val($("#DateStart").val());
        $("#EditDateEnd").val($("#DateEnd").val());
    });
}

function loadPartialViewCompute() {
    $("#computeView").empty();
    $("#computeView").load("/TrnDtr/Compute?payrollGroupId=" + $("#PayrollGroupId").val(), function (response, status, xhr) {
        $("#ComputeDateStart").val($("#DateStart").val());
        $("#ComputeDateEnd").val($("#DateEnd").val());
    });
}

function loadPartialViewProcess() {
    $("#processView").empty();
    $("#processView").load("/TrnDtr/Process?payrollGroupId=" + $("#PayrollGroupId").val(), function (response, status, xhr)
    {
        $("#ProcessDateStart").val($("#DateStart").val());
        $("#ProcessDateEnd").val($("#DateEnd").val());
    });
}

function addDays(date, number) {
    const newDate = new Date(date);
    return new Date(newDate.setDate(newDate.getDate() + number));
}

function onGridEdit(e) {
    var isAdmin = $("#isAdmin")[0].checked;
    var canEditDtrTime = $("#canEditDtrTime")[0].checked;
    var remarks = e.model.Dtrremarks || "";

    var isEditable = isAdmin || (canEditDtrTime && remarks.trim() != "");
    if (!isEditable) {
        e.container.find("input[name='TimeIn1']").attr("disabled", "disabled");
        e.container.find("input[name='TimeOut1']").attr("disabled", "disabled");
        e.container.find("input[name='TimeIn2']").attr("disabled", "disabled");
        e.container.find("input[name='TimeOut2']").attr("disabled", "disabled");
    }
}