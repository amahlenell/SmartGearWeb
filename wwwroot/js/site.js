// SmartGearWeb site interactivity (jQuery)

$(function () {
    // Confirm before submitting any delete form, instead of deleting instantly
    $("form.delete-form").on("submit", function (e) {
        var confirmed = confirm("Are you sure you want to delete this product? This cannot be undone.");
        if (!confirmed) {
            e.preventDefault();
        }
    });

    // Auto-dismiss the low-stock alert banner after 6 seconds
    setTimeout(function () {
        $(".low-stock-alert").fadeOut(400);
    }, 6000);
});
