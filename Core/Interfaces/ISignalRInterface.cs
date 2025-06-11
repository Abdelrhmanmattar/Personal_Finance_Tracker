using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISignalRService
    {
        Task SendMessageAll(string message);
        Task SendMessageUser(string UserID, string message);
        Task SendMessageGroup(string GroupID, string message);
        Task AddUsertoGroup(string UserID, string GroupID);

        Task RemoveUserfromGroup(string UserID, string GroupID);
        //Task<IReadOnlyList<string>> ConnectionIDs(string userID);
        //Task<string> LastConnectionID(string userID);
    }
}
