using System.Threading.Tasks;
using IdentityServer4.Models;

namespace ChatChainCommon.IdentityServerStore
{
    public interface ICustomClientStore
    {
        Task<Client> FindClientByIdAsync(string clientId);
        void RemoveClient(string clientId);
        void AddClient(Client client);
    }
}