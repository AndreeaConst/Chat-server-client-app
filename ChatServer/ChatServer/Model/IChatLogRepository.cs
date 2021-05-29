
using System.Collections.Generic;

namespace ChatServer.Server.Model
{
    public interface IChatLogRepository
    {
        void Add(ChatLog chatLog);
        IEnumerable<ChatLog> GetAll();
    }
}
