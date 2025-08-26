using Microsoft.AspNetCore.SignalR;
using NCoreWebApp.Dtos;

namespace NCoreWebApp.Hubs
{
    public class EisenHub : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendEisenUpdate(EisDto eisenItem)
        {
            await Clients.All.SendAsync("EisenUpdated", eisenItem);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}