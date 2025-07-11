//For Employee
$transitionTime = 200;

var grid = new Object();

$indexView = $(".indexView");
$detailView = $(".detailView");

$isDirty = false;
$isSaving = false;
$isTurnPage = false;
$isSearchFocused = false;
$isNewMemoLine = false;

$SelectedNavigationControl = "none";
$SelectedAction = "none";

$SelectedGridRecord = new Object();
$SelectedGridEmployeeMemo = new Object();

var isNewShiftCodeRowAdded = false;
var shiftCodesCurrentPage = 1;

window.addEventListener('beforeunload', function (e) {
    if ($isDirty) {
        e.preventDefault();
        e.returnValue = '';
    }
});

$(document).ready(function () {
    grid = $("#grid").data("kendoGrid");

    $("#loading").hide();

    $("#LastName").onblur = onNameLostFocus;
    $("#FirstName").onblur = onNameLostFocus;
    $("#MiddleName").onblur = onNameLostFocus;

    var id = getParameterByName("id");

    if (id != null)
    {
        $indexView.hide($transitionTime);
        $detailView.show($transitionTime);

        $detailView.removeAttr("hidden");

        loadPartialView(id);
    }

    var exportButton = document.getElementById("exportWithSalary");
    exportButton.addEventListener("click", function () {
        // Get the anti-forgery token value
        var token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        fetch('/MstEmployee/Index?handler=GenerateEmployeeExcel', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({}),
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.blob();
            })
            .then(blob => {
                var url = window.URL.createObjectURL(blob);
                var a = document.createElement('a');
                a.style.display = 'none';
                a.href = url;
                a.download = 'Employees.xlsx';
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                a.remove();
            })
            .catch(error => {
                console.error('Fetch error:', error);
            });
    });

    applyMobileSizesToControls();
});

function forgeryToken() {
    return {
        __RequestVerificationToken: kendo.antiForgeryTokens().__RequestVerificationToken,
        departmentId: $("#departmentCmb").val(),
        search: $("#searchTxt").val()
    };
}

//Button Commands and Lookup
function onDepartmentChange() {
    grid.dataSource.read();
}

function onClearDepartmentClick() {
    $("#departmentCmb").val("");
    $("#departmentCmb").data("kendoComboBox").text("");

    grid.dataSource.read();
}

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

function CmdDelete() {
    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/MstEmployee/Detail?handler=Delete",
        data: {
            __RequestVerificationToken: token,
            "id": $("#Id").val()
        },
        success: function (data) {
            GetWarningMessage("Record [" + $("#FullName").val() + "] is successfully deleted.");

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

function CmdApproveShiftCodes()
{
    if ($isDirty) {
        GetWarningMessage("Please save the changes before approving...")
    }
    else {
        var headers = {};

        headers["RequestVerificationToken"] = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        var frmData = GetFormData($("#frmDetail"));
        var dataValue = { ...frmData };
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();        

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/MstEmployee/Detail?handler=ApproveEmployeeShiftCodes",
            data:
            {
                __RequestVerificationToken: token,
                "employee": dataValue
            },
            success: function (data) {
                GetSaveMessage($("#FullName").val());

                $isDirty = false;
                $isSaving = true

                loadPartialView($("#Id").val());
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
}

function CmdApproveSalary() {

    if ($isDirty) {
        GetWarningMessage("Please save the changes of the new salary before approving...")
    }
    else
    {
        var headers = {};

        headers["RequestVerificationToken"] = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        var frmData = GetFormData($("#frmDetail"));
        var dataValue = { ...frmData };
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        if (dataValue.NewDailyRate === "0.00") {
            GetWarningMessage("New daily rate cannot be zero...")
            return;
        }

        if (dataValue.NewHourlyRate === "0.00") {
            GetWarningMessage("New hourly rate cannot be zero...")
            return;
        }

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/MstEmployee/Detail?handler=SaveEmployeeNewSalary",
            data:
            {
                __RequestVerificationToken: token,
                "employee": dataValue
            },
            success: function (data) {
                GetSaveMessage($("#FullName").val());

                $isDirty = false;
                $isSaving = true

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
}
function CmdSave()
{
    var headers = {};

    headers["RequestVerificationToken"] = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    var frmData = GetFormData($("#frmDetail"));

    frmData.OldPictureFilePath = $("#OldImage").val();
    frmData.PictureFilePath = document.forms.frmDetail.elements.Image.value;
    frmData.IsLocked = $("#IsLocked")[0].checked;
    frmData.SssisGrossAmount = $("#SssisGrossAmount")[0].checked;
    frmData.IsExemptedInMandatoryDeductions = $("#IsExemptedInMandatoryDeductions")[0].checked;
    frmData.IsFlex = $("#IsFlex")[0].checked;
    frmData.IsLongShift = $("#IsLongShift")[0].checked;
    frmData.IsFlexBreak = $("#IsFlexBreak")[0].checked;

    if ($("#IsSalaryConfidential")[0] != undefined) {
        frmData.IsSalaryConfidential = $("#IsSalaryConfidential")[0].checked;
    }
    else {
        frmData.IsSalaryConfidential = true;
    }

    var frmDataSF1 = { MstEmployeeMemos: GetGridViewJsonData($("#MstEmployeeMemos")) };
    var frmDataSF2 = { MstEmployeeShiftCodes: GetGridViewJsonData($("#MstEmployeeShiftCodes")) };
    var dataValue = { ...frmData, ...frmDataSF1, ...frmDataSF2 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    for (var item of frmDataSF1.MstEmployeeMemos) {
        if (item.MemoDate != null) {
            item.MemoDate = new Date(item.MemoDate).toLocaleDateString();
        }
    }

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/MstEmployee/Detail?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "employee": dataValue
        },
        success: function (data) {
            var fileUpload = $("#ImageFile").get(0);

            if (fileUpload != undefined)
            {
                var files = fileUpload.files;

                var formData = new FormData();

                for (var i = 0; i != files.length; i++) {
                    formData.append("files", files[i]);
                }

                formData.append("Id", $("#Id").val());

                $.ajax({
                    async: true,
                    headers: headers,
                    url: "/MstEmployee/Detail?handler=UploadImage",
                    data: formData,
                    processData: false,
                    contentType: false,
                    type: "POST",
                    success: function () {
                        GetSaveMessage($("#FullName").val());

                        $isDirty = false;
                        $isSaving = true

                        loadPartialView(data);
                        grid.dataSource.read();

                        $isTurnPage = false;
                    }
                });
            }
        },
        error: function (error) {
            GetErrorMessage(error, "Save");

            $isDirty = false;
            $isSaving = true
        }
    });
}

function CmdPreview()
{
    window.open(window.location.origin + "/MstEmployee/Report?paramId=" + $("#Id").val(), '_blank').focus();
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

//Memo Grid
function CmdAddMemo() {
    $isDirty = true;

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        url: "/MstEmployee/Memo?handler=Add",
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: {
            __RequestVerificationToken: token,
            employeeId: $("#Id").val()
        },
        success: function (data) {
            data.MemoDate = new Date(data.MemoDate).toLocaleDateString();

            $("#MstEmployeeMemos").getKendoGrid()
                .dataSource.insert(data);
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

function CmdDetailMemo(e)
{
    $isNewMemoLine = false;

    e.preventDefault();

    var subGrid = $("#MstEmployeeMemos").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    subGrid.select($(e.target).closest("tr"));

    $SelectedGridEmployeeMemo = item;

    loadPartialViewEmployeeMemo(item);

    $("#lineDetailModal").modal("show");
}

function CmdPrintMemo(e)
{
    var subGrid = $("#MstEmployeeMemos").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    if (item.Id == 0) {
        GetWarningMessage("Please save the employee record first before printing new Memos.");
    }
    else
    {
        window.open(window.location.origin + "/MstEmployee/MemoReport?paramId=" + item.Id, '_blank').focus();
    }
}

function CmdDeleteMemo(e) {
    $isDirty = true;

    var subGrid = $("#MstEmployeeMemos").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function CmdSaveEmployeeMemo()
{
    if ($isNewMemoLine == false)
    {
        $SelectedGridEmployeeMemo.set("MemoContent", $("#MemoContent").val());
    }

    $isDirty = true;
}

//Shift Code Grid
function CmdAddShiftCode() {
    //var data = $("#MstEmployeeShiftCodes").getKendoGrid()
    //    .dataSource.insert();
    //.dataSource.insert({ Id: 0, EmployeeId: $("#Id").val(), ShiftCodeId: null,  Status: "New", IsDeleted: false });

    var grid = $("#MstEmployeeShiftCodes").data("kendoGrid");
    grid.addRow();

    isNewShiftCodeRowAdded = true;

    if (isNewShiftCodeRowAdded) {
        var grid = $("#MstEmployeeShiftCodes").data("kendoGrid");
        var total = grid.dataSource.total();
        var pageSize = grid.dataSource.pageSize();
        var totalPages = Math.ceil(total / pageSize);

        if (total > pageSize && grid.dataSource.page() < totalPages) {
            grid.dataSource.page(totalPages);
        }
        isNewShiftCodeRowAdded = false; // Reset the flag
    }

    $isDirty = true;
}

function onShiftCodesPage(e) {
    shiftCodesCurrentPage = e.page;
}

function CmdDeleteShiftCode(e) {
    $isDirty = true;

    var subGrid = $("#MstEmployeeShiftCodes").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("Status", "Deleted");
    item.set("IsDeleted", true);

    //subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//User Input Events
function onDateChange(e)
{
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

function onNameLostFocus()
{
    onUpdateFullName();
}

function onUpdateFullName() {
    var lastName = $("#LastName").val();
    var firstName = $("#FirstName").val();
    var middleName = $("#MiddleName").val();

    $("#FullName").val(lastName + ", " + firstName + " " + middleName);
}

function calculateSalary()
{
    var frmData = GetFormData($("#frmDetail"));

    var monthlyRate = parseFloat(frmData.NewMonthlyRate.replace(/,/g, ''));
    var payrollRate = monthlyRate / 2;

    $("#NewPayrollRate").val(String(payrollRate));

    var dailyRate = payrollRate / 13;

    $("#NewDailyRate").val(String(dailyRate));
    $("#NewAbsentDailyRate").val(String(dailyRate));

    var hourlyRate = dailyRate / 8;

    $("#NewHourlyRate").val(String(hourlyRate));

    var nightHourlyRate = hourlyRate * 0.10;

    $("#NewNightHourlyRate").val(String(nightHourlyRate));

    $("#NewOvertimeHourlyRate").val(String(hourlyRate * 1.25));
    $("#NewOvertimeNightHourlyRate").val(String(nightHourlyRate * 2));

    $("#NewTardyHourlyRate").val(hourlyRate); 
}

function onFKShiftCodeChange(e)
{
    $isDirty = true;

    var subGrid = $("#MstEmployeeShiftCodes").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    if (item.Id == 0) {
        item.set("EmployeeId", $("#Id").val());
        item.set("Status", "New");
    }
    else
    {
        item.set("Status", "Modified");
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

function loadPartialView(id) {
    $("#loading").show();

    $("#detailFormView").empty();

    if (id == 0)
    {
        $("#detailFormView").load("/MstEmployee/Detail?handler=Add", function (response, status, xhr)
        {
            if (status == "error")
            {
                GetErrorMessage("error", "Add");
            }
            else
            {
                $("#loading").hide();
                tabStripSelectItem();
            }
        });
    }
    else
    {
        $("#detailFormView").load("/MstEmployee/Detail?Id=" + id, function (response, status, xhr)
        {
            if (status == "error")
            {
                GetErrorMessage("error", "Edit");
            }
            else
            {
                $("#loading").hide();
                tabStripSelectItem();
            }
        });
    }
}

function tabStripOnSelect(e)
{
    localStorage.setItem("employeeDetailSelectedTabStripItem", e.item.outerText);
}

function tabStripSelectItem()
{
    var text = localStorage.getItem("employeeDetailSelectedTabStripItem");
    var index = 0;

    if (text == "General Information")
    {
        index = 0;
    }

    if (text == "Payroll Information") {
        index = 1;
    }

    if (text == "Memo") {
        index = 2;
    }

    if (text == "Shift Codes") {
        index = 3;
    }

    if (text == "Salary History") {
        index = 4;
    }

    if (text == "New Salary") {
        index = 5;
    }

    var tabStrip = $("#itemTabs").data("kendoTabStrip");
    tabStrip.select(index);
}

function loadPartialViewViaTurnPage() {
    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/MstEmployee/Detail?handler=TurnPage",
        data: {
            __RequestVerificationToken: token,
            "id": $("#Id").val(),
            "departmentId": $("#departmentCmb").val(),
            "action": $SelectedAction
        },
        success: function (data) {
            if (data.Id != 0) {
                loadPartialView(data.Id);
            }
        }
    });
}

function loadPartialViewEmployeeMemo(item) {
    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $("#detailEmloyeeMemoView").empty();

    var data = JSON.parse(JSON.stringify(item));

    $("#detailEmloyeeMemoView").load("/MstEmployee/Memo?handler=Load",
        {
            __RequestVerificationToken: token,
            memo: data
        });   
}
