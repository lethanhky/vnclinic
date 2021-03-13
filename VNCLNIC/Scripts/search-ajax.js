
﻿function search(searchSelector, urlAction, inputGetValueSelector) {
    /// sử dụng autocomplete của jquery
    $(searchSelector).autocomplete({
        source: function (request, response) {
            $.ajax({
                url: urlAction,
                dataType: 'json',
                data: { search: $(searchSelector).val() },
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }));
                },
                error: function (xhr, status, error) {
                    alert("Error" + error);
                }
            });
        },
        select: function (e, i) {
            // sau khi search sẽ hiện ra các value, sau khi chọn sẽ update input được ẩn với value id
            $(searchSelector).val(i.item.label);
            $(inputGetValueSelector).val(i.item.val);
        },
        minLength: 3,
        change: function (event, ui) {
            // xóa value đi khi textbox thay đổi
           
            if (ui.item) {
                $(inputGetValueSelector).val(ui.item.val);
                toastr.success("Dữ liệu hợp lệ.", "", option);
            } else {
                $(inputGetValueSelector).val('');
                toastr.warning("Dữ liệu chưa hợp lệ.", "", option);
            }
        }
    });
    $(searchSelector).attr("autocomplete", "nope");
}

var option = toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "200",
    "hideDuration": "500",
    "timeOut": "1000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};
