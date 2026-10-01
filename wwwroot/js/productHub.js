// Connects to the SignalR ProductHub and shows a live banner whenever
// another user (or browser tab) adds, edits, or deletes a product —
// no page refresh needed to know the catalogue changed.

$(function () {
    if (typeof signalR === "undefined") {
        console.warn("SignalR script did not load; live updates disabled.");
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/products")
        .withAutomaticReconnect()
        .build();

    connection.on("ProductsChanged", function (message) {
        showLiveUpdateBanner(message);
    });

    connection.start().catch(function (err) {
        console.error("SignalR connection failed:", err.toString());
    });

    function showLiveUpdateBanner(message) {
        const $banner = $("<div>")
            .addClass("alert alert-info live-update-banner")
            .text(message + " Refresh to see the latest list.")
            .hide();

        $("main.pb-3").prepend($banner);
        $banner.fadeIn(300);

        setTimeout(function () {
            $banner.fadeOut(400, function () {
                $(this).remove();
            });
        }, 6000);
    }
});
