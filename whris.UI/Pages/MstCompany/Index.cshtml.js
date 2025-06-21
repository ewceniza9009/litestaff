//For Company
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

    if ($isDirty) {
        $("#confirmSaveDialog").data("kendoDialog").open();
        $("#showDialogBtn").fadeOut();
    }

    if (!$isDirty) {
        $indexView.show($transitionTime);
        $detailView.hide($transitionTime);
    }
})

function onGridDatabound(e) {
    if (grid.dataSource.data().length == 1) {

        if ($isSaving == true) {
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

function onCancelSave() {
    $isDirty = false;

    if ($SelectedNavigationControl == "lookup") {
        $indexView.show($transitionTime);
        $detailView.hide($transitionTime);
    }

    if ($SelectedNavigationControl == "turnPage") {
        loadPartialViewViaTurnPage();
    }
}

function onConfirmSave() {
    CmdSave();

    if ($SelectedNavigationControl == "lookup") {
        $indexView.show($transitionTime);
        $detailView.hide($transitionTime);
    }

    if ($SelectedNavigationControl == "turnPage") {
        loadPartialViewViaTurnPage();
    }
}

function CmdAdd() {
    $indexView.hide($transitionTime);
    $detailView.show($transitionTime);

    $detailView.removeAttr("hidden");

    loadPartialView(0);

    $SelectedGridRecord = null;
}

function CmdHome() {
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

function CmdDelete() {
    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/MstCompany/Detail?handler=Delete",
        data:
        {
            __RequestVerificationToken: token,
            "Id": $("#Id").val()
        },
        success: function (data) {
            GetWarningMessage("Record [" + $("#Company").val() + "] is successfully deleted.");

            $indexView.show($transitionTime);
            $detailView.hide($transitionTime);

            $isDirty = false;

            grid.dataSource.read();
        },
        error: function (error) {
            GetErrorMessage(error, "Delete");
        }
    });
}

function CmdSave() {
    var frmData = GetFormData($("#frmDetail"));

    frmData.OldImageLogo = $("#OldImage").val();
    frmData.ImageLogo = document.forms.frmDetail.elements.Image.value;
    frmData.MandatoryDeductionDivisor = parseInt($("#MandatoryDeductionDivisor").val());
    frmData.IsComputeNightOvertimeOnNonRegularDays = $("#IsComputeNightOvertimeOnNonRegularDays")[0].checked;
    frmData.IsHolidayPayLateDeducted = $("#IsHolidayPayLateDeducted")[0].checked;
    frmData.IsComputePhicByPercentage = $("#IsComputePhicByPercentage")[0].checked;
    frmData.IsLocked = $("#IsLocked")[0].checked;

    var frmDataSF1 = { MstBranches: GetGridViewJsonData($("#MstBranches")) };
    var dataValue = { ...frmData, ...frmDataSF1 };

    var headers = {};

    headers["RequestVerificationToken"] = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/MstCompany/Detail?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "company": dataValue
        },
        success: function (data) {
            var fileUpload = $("#ImageFile").get(0);
            var files = fileUpload.files;

            var formData = new FormData();

            for (var i = 0; i != files.length; i++) {
                formData.append("files", files[i]);
            }

            formData.append("Id", $("#Id").val());

            $.ajax({
                async: true,
                headers: headers,
                url: "/MstCompany/Detail?handler=UploadImage",
                data: formData,
                processData: false,
                contentType: false,
                type: "POST",
                success: function () {
                    GetSaveMessage($("#Company").val());

                    $isDirty = false;
                    $isSaving = true

                    loadPartialView(data);
                    grid.dataSource.read();

                    $isTurnPage = false;
                }
            });
        },
        error: function (error) {
            GetErrorMessage(error, "Save");

            $isDirty = false;
            $isSaving = true
        }
    });
}

function ImageFileChange() {
    $isDirty = true;

    var file = $("#ImageFile");
    var path = URL.createObjectURL(file[0].files[0]);

    $("#ImagePath").attr("src", path)

    document.forms.frmDetail.elements.Image.value = file[0].files[0].name;
}

function CmdBack() {
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
function CmdAddBranch(e) {
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/MstCompany/Detail?handler=AddBranch",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            companyId: $("#Id").val()
        },
        success: function (data) {

            $("#MstBranches").getKendoGrid()
                .dataSource.insert(data);
        }
    });
}

function CmdDeleteBranch(e) {
    $isDirty = true;

    var subGrid = $("#MstBranches").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//User Input Events
function onCheckboxChange() {
    $isDirty = true;
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

function loadPartialView(id) {
    $("#loading").show();

    $("#detailFormView").empty();

    if (id == 0) {
        $("#detailFormView").load("/MstCompany/Detail?handler=Add", function (response, status, xhr) {
            if (status == "error") {
                GetErrorMessage("error", "Add");
            }
            else {
                $("#loading").hide();
            }
        });
    }
    else {
        $("#detailFormView").load("/MstCompany/Detail?Id=" + id, function (response, status, xhr) {
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
        url: "/MstCompany/Detail?handler=TurnPage",
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
