
using System.Collections.Generic;

namespace ChatServer.Server.Model
{
    public interface IUsersRepository
    {
        void Add(Username username);
        void Delete(Username username);
        IEnumerable<Username> GetAll();
    }
}
