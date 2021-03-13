function img2Base64(element, displayedElement, hiddenField) {
    var file = element.files[0];
    var reader = new FileReader();
    reader.onloadend = function () {
        $("#" + displayedElement).attr("src", reader.result);
        $("#" + hiddenField).val(reader.result);
        //$("#"+displayedElement).text(reader.result);
    }
    reader.readAsDataURL(file);
}
function RotateImage(degree, img) {
    var rotation = parseInt($(degree).val());
    rotation = (rotation + 90) % 360;
    $(img).css({ 'transform': 'rotate(' + rotation + 'deg)' });
    $(degree).val(rotation);
}

function SaveRotateImage(degree) {
    try {
        $.ajax({
            type: "POST",
            dataType: "json",
            data: {
                path: degree.attr("name"),
                degree: degree.val(),
            },
            url: "/Resource/EZRotateImage",
            success: function (item) {
                if (item.success) {
                    toastr.success(item.message, "Success");
                    setTimeout(function () {
                        doCommit();
                    }, 1000);
                } else {
                    toastr.warning(item.message, "Failed");
                    setTimeout(function () {
                        doCommit();
                    }, 1000);
                }
            },
            error: function (err) {
                toastr.warning(item.message, "Failed");
                console.log(err);
            }
        });
    } catch (e) {
        toastr.warning("Cập nhật ảnh không thành công !", "Failed");
        location.reload();
    }
}
