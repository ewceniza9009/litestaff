//For DayType
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

function forgeryToken() {
    return {
        __RequestVerificationToken: kendo.antiForgeryTokens().__RequestVerificationToken,
        search: $("#searchTxt").val()
    };
}

//Button Commands and Lookup
function onSearchChange(e) {
    e.preventDefault();

    grid.dataSource.read();
}

function onClearSearchClick() {
    $("#searchTxt").val("");

    $indexView.show($transitionTime);
    $detailView.hide($transitionTime);

    grid.dataSource.read();
}

$("#searchTxt").on("click", function (e) {
    $("#loading").hide();

    $SelectedNavigationControl = "lookup";
    $isSearchFocused = true;

    if ($isDirty)
    {
        $("#confirmSaveDialog").data("kendoDialog").open();
        $("#showDialogBtn").fadeOut();
    }   

    if (!$isDirty)
    {
        $indexView.show($transitionTime);
        $detailView.hide($transitionTime);
    }
})

function onGridDatabound(e)
{
    if (grid.dataSource.data().length == 1) {

        if ($isSaving == true)
        {
            $indexView.show($transitionTime);
            $detailView.hide($transitionTime);

            $isSaving = false;

            return;
        }

        $indexView.hide($transitionTime);
        $detailView.show($transitionTime);

        $isSearchFocused = false;

        $detailView.removeAttr("hidden");

        if (grid.dataSource.view().length == 1) {
            $SelectedGridRecord = grid.dataSource.view()[0];
            loadPartialView(grid.dataSource.view()[0].Id);
        }  
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
        url: "/MstDayType/Detail?handler=Delete",
        data:
        {
            __RequestVerificationToken: token,
            "Id": $("#Id").val()
        },
        success: function (data) {
            GetWarningMessage("Record [" + $("#DayType").val() + "] is successfully deleted.");

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

    frmData.IsLocked = $("#IsLocked")[0].checked;

    var frmDataSF1 = { MstDayTypeDays: GetGridViewJsonData($("#MstDayTypeDays")) };
    var dataValue = { ...frmData, ...frmDataSF1 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    for (var item of frmDataSF1.MstDayTypeDays) {
        if (item.Date != null) {
            item.Date = new Date(item.Date).toLocaleDateString();
        }

        if (item.DateAfter) {
            item.DateAfter = new Date(item.DateAfter).toLocaleString();
        }

        if (item.DateBefore != null) {
            item.DateBefore = new Date(item.DateBefore).toLocaleString();
        }
    }

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/MstDayType/Detail?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "user": dataValue
        },
        success: function (data) {
            GetSaveMessage($("#DayType").val());

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

//Forms Grid
function CmdAddDayTypeDay(e)
{
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/MstDayType/Detail?handler=AddDayTypeDay",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            userId: $("#Id").val()
        },
        success: function (data) {
            var grid = $("#MstDayTypeDays").getKendoGrid();
            if (Array.isArray(data)) {
                data.forEach(function(item) {
                    item.Date = new Date(item.Date);
                    item.DateBefore = new Date(item.DateBefore);
                    item.DateAfter = new Date(item.DateAfter);
                    grid.dataSource.insert(item);
                });
            } else {
                data.Date = new Date(data.Date);
                data.DateBefore = new Date(data.DateBefore);
                data.DateAfter = new Date(data.DateAfter);
                grid.dataSource.insert(data);
            }
        }
    });
}

function CmdDeleteDayTypeDay(e)
{
    $isDirty = true;

    var subGrid = $("#MstDayTypeDays").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//DayType Input Events
function onEntityComboboxChange(e)
{
    $isDirty = true;
}

function onCheckboxChange()
{
    $isDirty = true;
}

function onBranchChange() { $isDirty = true; }
function onExcludedInFixedChange() { $isDirty = true; }
function onWithAbsentInFixedChange() { $isDirty = true; }


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
        $("#detailFormView").load("/MstDayType/Detail?handler=Add", function (response, status, xhr) {
            if (status == "error") {
                GetErrorMessage("error", "Add");
            }
            else {
                $("#loading").hide();
            }
        });
    }
    else
    {
        $("#detailFormView").load("/MstDayType/Detail?Id=" + id, function (response, status, xhr) {
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
        url: "/MstDayType/Detail?handler=TurnPage",
        data: {
            __RequestVerificationToken: token,
            "id": $("#Id").val(),
            "action": $SelectedAction
        },
        success: function (data) {
            if (data.Id != 0) {
                loadPartialView(data.Id);
            }
        }
    });
}

function CmdOpenBulkAddModal() {
    $("#bulkAddDialog").data("kendoDialog").open();
    if (!$("#bulkDate").data("kendoDatePicker")) {
        $("#bulkDate").kendoDatePicker({
            value: new Date(),
            format: "MM/dd/yyyy",
            change: function() {
                var date = this.value();
                if (date) {
                    var dateBefore = new Date(date);
                    dateBefore.setDate(date.getDate() - 1);
                    $("#bulkDateBefore").data("kendoDatePicker").value(dateBefore);
                    
                    var dateAfter = new Date(date);
                    dateAfter.setDate(date.getDate() + 1);
                    $("#bulkDateAfter").data("kendoDatePicker").value(dateAfter);
                }
            }
        });
        
        var defaultDate = new Date();
        var defaultBefore = new Date(defaultDate);
        defaultBefore.setDate(defaultDate.getDate() - 1);
        var defaultAfter = new Date(defaultDate);
        defaultAfter.setDate(defaultDate.getDate() + 1);

        $("#bulkDateBefore").kendoDatePicker({
            value: defaultBefore,
            format: "MM/dd/yyyy"
        });
        
        $("#bulkDateAfter").kendoDatePicker({
            value: defaultAfter,
            format: "MM/dd/yyyy"
        });
    }
}

function onConfirmBulkAdd() {
    var datePicker = $("#bulkDate").data("kendoDatePicker");
    var selectedDate = datePicker.value();
    
    var dateBeforePicker = $("#bulkDateBefore").data("kendoDatePicker");
    var selectedDateBefore = dateBeforePicker.value();
    
    var dateAfterPicker = $("#bulkDateAfter").data("kendoDatePicker");
    var selectedDateAfter = dateAfterPicker.value();
    
    if (!selectedDate || !selectedDateBefore || !selectedDateAfter) {
        alert("Please select all dates.");
        return false;
    }

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/MstDayType/Detail?handler=BulkAddDayTypeDay",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            userId: $("#Id").val(),
            date: kendo.toString(selectedDate, "yyyy-MM-dd"),
            dateBefore: kendo.toString(selectedDateBefore, "yyyy-MM-dd"),
            dateAfter: kendo.toString(selectedDateAfter, "yyyy-MM-dd")
        },
        success: function (data) {
            var grid = $("#MstDayTypeDays").getKendoGrid();
            if (Array.isArray(data)) {
                data.forEach(function(item) {
                    item.Date = new Date(item.Date);
                    item.DateBefore = new Date(item.DateBefore);
                    item.DateAfter = new Date(item.DateAfter);
                    grid.dataSource.insert(item);
                });
            }
            $isDirty = true;
        }
    });
    
    return true;
}
