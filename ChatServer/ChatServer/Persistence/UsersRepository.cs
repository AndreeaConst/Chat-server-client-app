
using ChatServer.Server.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;

namespace ChatServer.Server.Persistence
{
    [Export(typeof(IUsersRepository))]
    public class UsersRepository : IUsersRepository
    {
        private readonly List<Username> m_storage = new List<Username>();

        public void Add(Username user)
        {
            m_storage.Add(user);
        }
        public void Delete(Username user)
        {
            m_storage.Remove(user);
        }

        public IEnumerable<Username> GetAll()
        {
            return m_storage.AsReadOnly();
        }
    }
}
