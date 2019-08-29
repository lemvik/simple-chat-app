using System.Threading.Tasks;
using LemVic.Services.Chat.DataAccess.Models;

namespace LemVic.Services.Chat.Services
{
    public interface ITokenService
    {
        Task<string> Create(string userName, string alias);
    }
}
