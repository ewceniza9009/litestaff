//For User
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

function CmdDelete()
{
    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/MstUser/Detail?handler=Delete",
        data:
        {
            __RequestVerificationToken: token,
            "Id": $("#Id").val()
        },
        success: function (data) {
            GetWarningMessage("Record [" + $("#FullName").val() + "] is successfully deleted.");

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

    frmData.IsAdmin = $("#IsAdmin")[0].checked;
    frmData.CanEditDtrTime = $("#CanEditDtrTime")[0].checked;
    frmData.IsLocked = $("#IsLocked")[0].checked;

    var frmDataSF1 = { MstUserForms: GetGridViewJsonData($("#MstUserForms")) };
    var dataValue = { ...frmData, ...frmDataSF1 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/MstUser/Detail?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "user": dataValue
        },
        success: function (data) {
            GetSaveMessage($("#FullName").val());

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
function CmdAddForm(e)
{
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/MstUser/Detail?handler=AddUserForm",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            userId: $("#Id").val()
        },
        success: function (data) {

            $("#MstUserForms").getKendoGrid()
                .dataSource.insert(data);
        }
    });
}

function CmdDeleteForm(e)
{
    $isDirty = true;

    var subGrid = $("#MstUserForms").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function CmdDeleteAllLines(e) {
    $isDirty = true;

    var subGrid = $("#MstUserForms").getKendoGrid();
    var subGridData = $("#MstUserForms").getKendoGrid().dataSource.data();

    for (let i = 0; i < subGridData.length; i++) {
        subGridData[i].set("IsDeleted", true);
    }

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function CopyUserForms(e)
{
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/MstUser/Detail?handler=CopyUserForms",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            userId: $("#CopyUserId").val()
        },
        success: function (data)
        {
            var subGrid = $("#MstUserForms").getKendoGrid();
            var subGridData = $("#MstUserForms").getKendoGrid().dataSource.data();

            if (subGridData.length > 0) {
                const response = confirm("Pressing yes will delete all current line items in the grid, Do you want to continue?");

                if (response) {
                    for (let i = 0; i < subGridData.length; i++) {
                        subGridData[i].set("IsDeleted", true);
                    }

                    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });

                    data.forEach(item => {
                        $("#MstUserForms").getKendoGrid()
                            .dataSource.insert(item);
                    });
                }
            }
            else
            {
                data.forEach(item => {
                    $("#MstUserForms").getKendoGrid()
                        .dataSource.insert(item);
                });
            }
        }
    });
}

//User Input Events
function onEntityComboboxChange(e)
{
    $isDirty = true;
}

function onCheckboxChange()
{
    $isDirty = true;
}

function onFormChange() { $isDirty = true; }
function onViewFormChange() { $isDirty = true; }
function onEditFormChange() { $isDirty = true; }
function onAddFormChange() { $isDirty = true; }
function onDeleteFormChange() { $isDirty = true; }
function onLockFormChange() { $isDirty = true; }
function onPrintFormChange() { $isDirty = true; }

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
        $("#detailFormView").load("/MstUser/Detail?handler=Add", function (response, status, xhr)
        {
            if (status == "error")
            {
                GetErrorMessage("error", "Add");
            }
            else
            {
                $("#loading").hide();
            }
        });
    }
    else
    {
        $("#detailFormView").load("/MstUser/Detail?Id=" + id, function (response, status, xhr)
        {
            if (status == "error")
            {
                GetErrorMessage("error", "Edit");
            }
            else
            {
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
        url: "/MstUser/Detail?handler=TurnPage",
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
