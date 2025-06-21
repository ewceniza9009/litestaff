//For Mandatory Deductions
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

    var frmDataSF1 = { MstTableSsses: GetGridViewJsonData($("#MstTableSsses")) };
    var frmDataSF2 = { MstTableHdmfs: GetGridViewJsonData($("#MstTableHdmfs")) };
    var frmDataSF3 = { MstTablePhics: GetGridViewJsonData($("#MstTablePhics")) };
    var frmDataSF4 = { MstTableWtaxSemiMonthlies: GetGridViewJsonData($("#MstTableWtaxSemiMonthlies")) };
    //var frmDataSF5 = { MstTableWtaxMonthlies: GetGridViewJsonData($("#MstTableWtaxMonthlies")) };
    var frmDataSF5 = GetGridViewJsonData($("#MstTableWtaxMonthlies"));
    var frmDataSF6 = { MstTableWtaxYearlies: GetGridViewJsonData($("#MstTableWtaxYearlies")) };
    var frmDataSF7 = { MstTaxCodes: GetGridViewJsonData($("#MstTaxCodes")) };

    var dataValue = { ...frmData, ...frmDataSF1, ...frmDataSF2, ...frmDataSF3, ...frmDataSF4, ...frmDataSF6, ...frmDataSF7 };

    var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

    $isSaving = true

    $.ajax({
        async: true,
        type: "POST",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        url: "/MstMandatoryDeductionTable/Index?handler=Save",
        data:
        {
            __RequestVerificationToken: token,
            "mandatoryTaxTables": dataValue,
            "monthlies": JSON.stringify(frmDataSF5)
        },
        success: function (data) {
            GetSaveMessage("Mandatory deduction tables");

            $isDirty = false;
        },
        error: function (error) {
            GetErrorMessage(error, "Save");
        }
    });
}

function CmdHome() {
    window.open(window.location.origin).focus();
}

//SSS Grid
function CmdAddSss() {
    $isDirty = true;

    $("#MstTableSsses").getKendoGrid().dataSource.insert();
}

function CmdDeleteSss(e) {
    $isDirty = true;

    var subGrid = $("#MstTableSsses").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//HDMF Grid
function CmdAddHdmf() {
    $isDirty = true;

    $("#MstTableHdmfs").getKendoGrid().dataSource.insert();
}

function CmdDeleteHdmf(e) {
    $isDirty = true;

    var subGrid = $("#MstTableHdmfs").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//PHIC Grid
function CmdAddPhic()
{
    $isDirty = true;

    $("#MstTablePhics").getKendoGrid().dataSource.insert();
}

function CmdDeletePhic(e)
{
    $isDirty = true;

    var subGrid = $("#MstTablePhics").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Semi Grid
function CmdAddSemi()
{
    $isDirty = true;

    $("#MstTableWtaxSemiMonthlies").getKendoGrid().dataSource.insert();
}

function CmdDeleteSemi(e)
{
    $isDirty = true;

    var subGrid = $("#MstTableWtaxSemiMonthlies").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Monthly Grid
function CmdAddMonthly()
{
    $isDirty = true;

    $("#MstTableWtaxMonthlies").getKendoGrid().dataSource.insert();
}

function CmdDeleteMonthly(e)
{
    $isDirty = true;

    var subGrid = $("#MstTableWtaxMonthlies").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Yearly Grid
function CmdAddYearly()
{
    $isDirty = true;

    $("#MstTableWtaxYearlies").getKendoGrid().dataSource.insert();
}

function CmdDeleteYearly(e)
{
    var subGrid = $("#MstTableWtaxYearlies").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

//Tax Code
function CmdAddTaxCode()
{
    $isDirty = true;

    $("#MstTaxCodes").getKendoGrid().dataSource.insert();
}

function CmdDeleteTaxCode(e)
{
    $isDirty = true;

    var subGrid = $("#MstTaxCodes").getKendoGrid();
    var item = subGrid.dataItem($(e.target).closest("tr"));

    item.set("IsDeleted", true);

    subGrid.dataSource.filter({ field: "IsDeleted", operator: "eq", value: false });
}

function tabStripOnSelect(e) {
    localStorage.setItem("mandatorySelectedTabStripItem", e.item.outerText);
}

function tabStripSelectItem() {
    var text = localStorage.getItem("mandatorySelectedTabStripItem");
    var index = 0;

    if (text == "SSS") {
        index = 0;
    }

    if (text == "HDMF") {
        index = 1;
    }

    if (text == "PHIC") {
        index = 2;
    }

    if (text == "Semi-Monthly Withholding Tax") {
        index = 3;
    }

    if (text == "Monthly Withholding Tax") {
        index = 4;
    }

    if (text == "Yearly Withholding Tax") {
        index = 5;
    }

    var tabStrip = $("#itemTabs").data("kendoTabStrip");
    tabStrip.select(index);
}