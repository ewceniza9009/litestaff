

function CmdDelete()
{
    var response = confirm("Do you want to delete all transaction and calendar records.");

    if (response) {
        var token = $('input[name="__RequestVerificationToken"]', $("#frmDetail")).val();

        $.ajax({
            async: true,
            type: "POST",
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            url: "/SysUtilities/Index?handler=Delete",
            data:
            {
                __RequestVerificationToken: token,
                "Id": $("#Id").val()
            },
            success: function (data) {
                var messageBox = new jBox("Notice", {
                    attributes: {
                        x: "right",
                        y: "bottom"
                    },
                    //stack: !1,
                    animation: {
                        open: "pulse",
                        close: "zoomIn"
                    },
                    delayOnHover: !0,
                    showCountdown: !0,
                    color: "yellow",
                    title: "WHRIS",
                    content: "All transactions are deleted successfully"
                });

                messageBox.open();
            },
            error: function (error) {
                GetErrorMessage(error, "Delete");
            }
        });
    }    
}


$("#btnUploadEmployees").click(function (e)
{
    e.preventDefault();

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
        url: "/SysUtilities/Index?handler=UploadEmployees",
        success: function (result) {
            $("#processDetailModal").modal("hide");

            var messageBox = new jBox("Notice", {
                attributes: {
                    x: "right",
                    y: "bottom"
                },
                animation: {
                    open: "pulse",
                    close: "zoomIn"
                },
                delayOnHover: !0,
                showCountdown: !0,
                color: "blue",
                title: "WHRIS",
                content: "Successfully uploaded..."
            });

            messageBox.open();
        },
        error: function (xhr, status, error) {
            GetWarningMessage("Error occured, It's either server is unresponsive or there was no file uploaded.");
        }
    });
});
$("#btnSave").click(function (e)
{
    e.preventDefault();
    location.href = "/SysUtilities/Index?handler=DownloadZip" + this.value;
});

$("#btnShow").click(function (e) {
    $("#btnProcessSpin").removeAttr("hidden")

    $("#btnProcessGear").hide();
    $("#btnProcessSpin").show();
});

$("#btnHide").click(function (e) {

    $("#btnProcessGear").show();
    $("#btnProcessSpin").hide();
});

function LoadForm()
{
    //$("#btnProcessSpin").hide();
}