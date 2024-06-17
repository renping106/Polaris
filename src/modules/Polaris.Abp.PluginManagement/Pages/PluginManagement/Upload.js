$(document).ready(function () {
    $(document).on("change", "#UploadFile_File", function () {
        var fileName = $(this)[0].files[0].name;
        $("#UploadFile_Name").val(fileName);
    })
});