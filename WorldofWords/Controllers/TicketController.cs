using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.API.Models;
using System.Threading.Tasks;
using WorldofWords.Hubs;
using Microsoft.AspNet.SignalR;
using System.Threading;

namespace WorldofWords.Controllers
{
    [RoutePrefix("api/Ticket")]
    [WowAuthorization(AllRoles = new[] { "Admin", "Teacher", "Student" })]
    public class TicketController : BaseController
    {
        private ITicketService ticketService;

        public TicketController(ITicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        public async Task<IEnumerable<TicketForListModel>> GetTickets()
        {
            return await ticketService.GetAllTicketsAsync();
        }

        [Route("UserTickets")]
        public async Task<IEnumerable<TicketForListModel>> GetTicketsByUserIdAsync()
        {
            return await ticketService.GetTicketsByUserIdAsync(UserId);
        }


        [HttpGet]
        [Route("GetTicketsToTeacher")]
        public async Task<IEnumerable<TicketForListModel>> GetTicketsToTeacherAsync()
        {
            return await ticketService.GetTicketsToTeacherAsync(UserId);
        }

        public async Task<IHttpActionResult> CreateTicket(RequestFromUserModel model)
        {
            model.OwnerId = UserId;
            await ticketService.AddTicketAsync(model);
            return Ok();
        }

        [HttpPut]
        public async Task<int> UpdateTicketAsync(TicketForListModel ticket)
        {
            int result = await ticketService.UpdateAsync(ticket);
            return result;
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAsync(int id, int ownerId)
        {
            int i = id;
            bool isRemoved = await ticketService.RemoveAsync(id);
            return isRemoved
                ? Ok() as IHttpActionResult
                : BadRequest() as IHttpActionResult;
        }

        [Route("UserAmountOfUnread")]
        public async Task<int> GetAmountOfUnreadTicketForUser()
        {
            return await ticketService.GetAmountOfUnreadTicketForUser(UserId);
        }

        [Route("AdminAmountOfUnread")]
        public async Task<int> GetAmountOfUnreadTicketForAdmin()
        {
            return await ticketService.GetAmountOfUnreadTicketForAdmin();
        }
    }
}