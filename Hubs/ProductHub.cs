using Microsoft.AspNetCore.SignalR;

namespace SmartGearWeb.Hubs
{
    // Clients connect here over a persistent connection so the server can
    // push live notifications when the product catalogue changes, instead
    // of the browser needing to poll or the user needing to refresh.
    public class ProductHub : Hub
    {
    }
}
