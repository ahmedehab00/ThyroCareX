using System.Collections.Generic;
using System.Threading.Tasks;
using ThyroCareX.Data.Models;

namespace ThyroCareX.Service.Abstarct
{
    public interface IChatService
    {
        Task SaveMessageAsync(Message message);
        Task<List<Message>> GetChatHistoryAsync(string user1Id, string user2Id);
        Task MarkAsReadAsync(string receiverId, string senderId);
    }
}
