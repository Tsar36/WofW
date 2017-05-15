using System.Collections.Generic;
using System.Threading.Tasks;
using WorldOfWords.API.Models;
using WorldOfWords.API.Models.Models;

namespace WorldOfWords.Domain.Services.IServices
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketForListModel>> GetAllTicketsAsync();
        Task<IEnumerable<TicketForListModel>> GetTicketsByUserIdAsync(int id);
        Task<IEnumerable<TicketForListModel>> GetTicketsToTeacherAsync(int id);
        Task<TicketForListModel> GetTicketByIdAsync(int id);
        Task<int> AddTicketAsync(RequestFromUserModel ticket);
        Task<int> UpdateAsync(TicketForListModel ticket);
        Task<bool> RemoveAsync(int id);
        Task<int> GetAmountOfUnreadTicketForAdmin();
        Task<int> GetAmountOfUnreadTicketForUser(int userId);
    }
}
