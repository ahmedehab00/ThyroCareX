using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ThyroCareX.Data.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ThyroCareX.Core.Hubs
{
    public class ChatHub : Hub
    {
        // Dictionary to track online users: Key is UserId (as string), Value is set of ConnectionIds
        private static readonly ConcurrentDictionary<string, HashSet<string>> OnlineUsers = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                var connections = OnlineUsers.GetOrAdd(userId, _ => new HashSet<string>());
                lock (connections)
                {
                    connections.Add(Context.ConnectionId);
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                if (OnlineUsers.TryGetValue(userId, out var connections))
                {
                    lock (connections)
                    {
                        connections.Remove(Context.ConnectionId);
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string senderId, string receiverId, string content, string? imageUrl, string senderType)
        {
            // Prepare the message object (to be sent to client)
            var message = new
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                ImageUrl = imageUrl,
                SenderType = senderType,
                SentAt = DateTime.UtcNow
            };

            // Send to receiver if online
            if (OnlineUsers.TryGetValue(receiverId, out var connections))
            {
                List<string> connectionList;
                lock (connections)
                {
                    connectionList = new List<string>(connections);
                }
                
                foreach (var connectionId in connectionList)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
                }
            }

            // Also send back to other connections of the sender (to sync multiple tabs)
            // But skip the current connection (caller) to prevent duplication
            if (OnlineUsers.TryGetValue(senderId, out var senderConnections))
            {
                List<string> senderConnectionList;
                lock (senderConnections)
                {
                    senderConnectionList = new List<string>(senderConnections);
                }

                foreach (var connectionId in senderConnectionList)
                {
                    if (connectionId != Context.ConnectionId)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
                    }
                }
            }
        }
    }
}
