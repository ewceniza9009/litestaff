//For Tables
$transitionTime = 200;

var grid = new Object();

$isDirty = false;
$isSaving = false;

window.addEventListener('beforeunload', function (e) {
    if ($isDirty) {
        e.preventDefault();
        e.returnValue = '';
    }
});

$(document).ready(function () {
    tabStripSelectItem();
});

//Button Commands and Lookup
function CmdSave()
{
    var frmData = GetFormData($("#frmDetail"));

    var frmDataSF1 = { MstPayrollGroups: GetGridViewJsonData($("#MstPayrollGroups")) };
    var frmDataSF2 = { MstPayrollTypes: GetGridViewJsonData($("#MstPayrollTypes")) };
    var frmDataSF3 = { MstDepartments: GetGridViewJsonData($("#MstDepartments")) };
    var frmDataSF4 = { MstPositions: GetGridViewJsonData($("#MstPositions")) };
    var frmDataSF5 = { MstAccounts: GetGridViewJsonData($("#MstAccounts")) };
    var frmDataSF6 = { MstReligions: GetGridViewJsonData($("#MstReligions")) };
    var frmDataSF7 = { MstCitizenships: GetGridViewJsonData($("#MstCitizenships")) };
    var frmDataSF8 = { MstZipCodes: GetGridViewJsonData($("#MstZipCodes")) };
    var frmDataSF9 = { MstDivisions: GetGridViewJsonData($("#MstDivisions")) };

    var dataValue = { ...frmData, ...frmDataSF1, ...frmDataSF2, ...frmDataSF3, ...frmDataSF4, ...frmDataSF5, ...frmDataSF6, ...frmDataSF7, ...frmDataSF8, ...frmDataSF9 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $isSaving = true

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/SysTables/Index?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "tables": dataValue
        },
        success: function (data) {
            GetSaveMessage("System tables");

            $isDirty = false;
        },
        error: function (error) {
            GetErrorMessage(error, "Save");

            $isDirty = false;
        }
    });
}

function CmdHome() {
    window.open(window.location.origin).focus();
}

//Payroll Group
function CmdAddPayrollGroup() {
    $isDirty = true;

    $("#MstPayrollGroups").getKendoGrid().dataSource.insert();
}

function CmdDeletePayrollGroup(e) {
    $isDirty = true;

    var subGrid = $("#MstPayrollGroups").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Payroll Type
function CmdAddPayrollType() {
    $isDirty = true;

    $("#MstPayrollTypes").getKendoGrid().dataSource.insert();
}

function CmdDeletePayrollType(e) {
    $isDirty = true;

    var subGrid = $("#MstPayrollTypes").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Department
function CmdAddDepartment()
{
    $isDirty = true;

    $("#MstDepartments").getKendoGrid().dataSource.insert();
}

function CmdDeleteDepartment(e)
{
    $isDirty = true;

    var subGrid = $("#MstDepartments").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Position
function CmdAddPosition()
{
    $isDirty = true;

    $("#MstPositions").getKendoGrid().dataSource.insert();
}

function CmdDeletePosition(e)
{
    $isDirty = true;

    var subGrid = $("#MstPositions").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Account
function CmdAddAccount()
{
    $isDirty = true;

    $("#MstAccounts").getKendoGrid().dataSource.insert();
}

function CmdDeleteAccount(e)
{
    $isDirty = true;

    var subGrid = $("#MstAccounts").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Religion
function CmdAddReligion()
{
    $isDirty = true;

    $("#MstReligions").getKendoGrid().dataSource.insert();
}

function CmdDeleteReligion(e)
{
    var subGrid = $("#MstReligions").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Citizenship
function CmdAddCitizenship()
{
    $isDirty = true;

    $("#MstCitizenships").getKendoGrid().dataSource.insert();
}

function CmdDeleteCitizenship(e)
{
    $isDirty = true;

    var subGrid = $("#MstCitizenships").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Zip Code
function CmdAddZipCode() {
    $isDirty = true;

    $("#MstZipCodes").getKendoGrid().dataSource.insert();
}

function CmdDeleteZipCode(e) {
    $isDirty = true;

    var subGrid = $("#MstZipCodes").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Division
function CmdAddDivision() {
    $isDirty = true;

    $("#MstDivisions").getKendoGrid().dataSource.insert();
}

function CmdDeleteDivision(e) {
    $isDirty = true;

    var subGrid = $("#MstDivisions").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function tabStripOnSelect(e) {
    localStorage.setItem("tablesSelectedTabStripItem", e.item.outerText);
}

function tabStripSelectItem() {
    var text = localStorage.getItem("tablesSelectedTabStripItem");
    var index = 0;

    if (text == "Payroll Group") {
        index = 0;
    }

    if (text == "Payroll Type") {
        index = 1;
    }

    if (text == "Department") {
        index = 2;
    }

    if (text == "Position") {
        index = 3;
    }

    if (text == "Chart of Accounts") {
        index = 4;
    }

    if (text == "Religion") {
        index = 5;
    }

    if (text == "Citizenship") {
        index = 6;
    }

    if (text == "Zip Code") {
        index = 7;
    }

    if (text == "Division") {
        index = 8;
    }

    var tabStrip = $("#itemTabs").data("kendoTabStrip");
    tabStrip.select(index);
}