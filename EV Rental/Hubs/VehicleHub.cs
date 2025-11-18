using Microsoft.AspNetCore.SignalR;
using DataAccessLayer.Entities;

namespace EV_Rental.Hubs
{
    public class VehicleHub : Hub
    {
        public async Task SendVehicleUpdate(string action, Vehicle vehicle)
        {
            await Clients.All.SendAsync("ReceiveVehicleUpdate", action, vehicle);
        }

        public async Task NotifyVehicleSearch(int totalResults)
        {
            await Clients.All.SendAsync("ReceiveSearchResults", totalResults);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        }
    }
}
