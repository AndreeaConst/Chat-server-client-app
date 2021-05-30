using ChatServer;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;

namespace ChatServer.Server.Model
{
    [Export]
    public class UsersServiceModel
    {
        [Import]
        private Logger m_logger = null;

        [Import]
        private IUsersRepository m_repository = null;
        private event Action<Username> Added;
        private event Action<Username> Deleted;

        public void Add(Username user)
        {
            m_logger.Info($"{user}");

            m_repository.Add(user);
            Added?.Invoke(user);
        }

        public void Delete(Username user)
        {
            m_logger.Info($"{user}");

            m_repository.Delete(user);
            Deleted?.Invoke(user);
        }

        public IObservable<Username> GetUsersAsObservable()
        {
            var oldLogs = m_repository.GetAll().ToObservable();
            var newLogs = Observable.FromEvent<Username>((x) => Added += x, (x) => Added -= x);

            return oldLogs.Concat(newLogs);
        }
    }
}
