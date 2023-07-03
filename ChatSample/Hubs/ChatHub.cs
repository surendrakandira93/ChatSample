using ChatSample.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatSample.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;
        public ChatHub(IChatService _chatService)
        {
            this.chatService = _chatService;
        }

        public override async Task OnConnectedAsync()
        {

            var httpContext = this.Context.GetHttpContext();
            var name = httpContext.Request.Query["name"].ToString();
            var userId = chatService.AddUser(name);
            var contextId = Context.ConnectionId;
            var connectedUsers = chatService.KeepUserConnection(userId.ToString(), contextId);
            await Clients.All.SendAsync("userConnected", userId, name);
            var allUsers = chatService.GetAllUser();
            await Clients.All.SendAsync("allConnectedUsers", allUsers);

            var allGroup = chatService.GetAllGroup();
            await Clients.All.SendAsync("getAllGroups", allGroup);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var contextId = Context.ConnectionId;
            var connectedUsers = chatService.RemoveUserConnection(contextId);
            var allUsers = chatService.GetAllUser();
            await Clients.All.SendAsync("allConnectedUsers", allUsers);
            await base.OnDisconnectedAsync(ex);
        }

        public async Task CreateGroup(string groupName, int userId)
        {
            //var userFrom = chatService.GetUserById(userId);
            int groupId = chatService.AddGroup(groupName);
            // chatService.AddGroupMember(groupId, userId);
            // var connectionIds = chatService.GetUserConnections(userId.ToString());
            //foreach (var connectionId in connectionIds)
            //{
            //    await Groups.AddToGroupAsync(connectionId, groupId.ToString());
            //}
            //await Clients.Clients(connectionIds).SendAsync("userAddedToGroup", userFrom.Value, groupName, groupId.ToString());
            await Clients.All.SendAsync("appendNewGroups", groupId, groupName);
        }

        public async Task JoinGoup(int userId, int groupId)
        {
            string groupName = chatService.GetGroupById(groupId).Name;
            var userFrom = chatService.GetUserById(userId);
            chatService.AddGroupMember(groupId, userId);            
            var connectionIds = chatService.GetUserConnections(userId.ToString());
            foreach (var connectionId in connectionIds)
            {
                await Groups.AddToGroupAsync(connectionId, groupId.ToString());
            }
            await Clients.Clients(connectionIds).SendAsync("userAddedToGroup", userFrom.Value, groupName, groupId.ToString());           
        }

        public async Task RemoveGroup(int userId, int group)
        {
            var connectionIds = chatService.GetUserConnections(userId.ToString());
            foreach (var connectionId in connectionIds)
            {
                await Groups.RemoveFromGroupAsync(connectionId, group.ToString());
            }
        }

        public async Task Send(string name, string message)
        {
            await Clients.All.SendAsync("broadcastMessage", name, message);
        }

        public async Task SendToUser(int sendFrom, int sendTo, string message)
        {
            // Call the broadcastMessage method to update clients.
            var userFrom = chatService.GetUserById(sendFrom);
            var userTo = chatService.GetUserById(sendTo);
            var connectionIds = chatService.GetUserConnections(sendFrom.ToString());
            if (connectionIds.Any())
            {
                await Clients.Clients(connectionIds).SendAsync("broadcastToUser", sendFrom, userFrom.Value, sendTo, userTo.Value, message);
            }

            var connectionIds1 = chatService.GetUserConnections(sendTo.ToString());
            if (connectionIds1.Any())
            {
                await Clients.Clients(connectionIds1).SendAsync("broadcastToUser", sendTo, userFrom.Value, sendFrom, userTo.Value, message);
            }

        }

        public async Task SendMessageToGroup(int groupId, int senderid, string sendername, string message)
        {
            await Clients.Group(groupId.ToString()).SendAsync("receiveMessageToGroup", senderid, sendername, groupId, message);
        }
    }
}