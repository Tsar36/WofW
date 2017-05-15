using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.Domain.Models;
using WorldOfWords.Domain.Services.IServices;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using System.Data.Entity;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.API.Models;

namespace WorldOfWords.Domain.Services.Services
{
    public class TicketService : ITicketService
    {
        private IUnitOfWorkFactory unitOfWorkFactory;
        private ITicketMapper ticketMapper;

        public TicketService(IUnitOfWorkFactory unitOfWorkFactory, ITicketMapper ticketMapper)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.ticketMapper = ticketMapper;
        }

        public async Task<IEnumerable<TicketForListModel>> GetAllTicketsAsync()
        {
            List<Ticket> tickets;
            using(var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                tickets = await uow.TicketRepository.GetAll().Include(t => t.User).ToListAsync();
            }
            return tickets.Select(t => ticketMapper.Map(t));
        }

        public async Task<IEnumerable<TicketForListModel>> GetTicketsByUserIdAsync(int id)
        {
            List<Ticket> tickets;

            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                tickets = await uow.TicketRepository.GetAll().Include(t => t.User).Where(t=>t.OwnerId == id).ToListAsync();
            }

            return tickets.Select(t => ticketMapper.Map(t));
        }
        
        public async Task<IEnumerable<TicketForListModel>> GetTicketsToTeacherAsync(int id)
        {
            List<Ticket> tickets;
            using(var uow=unitOfWorkFactory.GetUnitOfWork())
            {
                tickets = await uow.TicketRepository.GetAll().Include(t => t.User).Where(t => t.Subject == "SubscriptionRequest")
                   .Where(t => t.Group.OwnerId == id)
                   .Where(t => t.ReviewStatus == 0 ||
                               t.ReviewStatus == 1)
                   .ToListAsync();
            }
            return tickets.Select(t => ticketMapper.Map(t));
        }

        public async Task<TicketForListModel> GetTicketByIdAsync(int id)
        {
            Ticket ticket;

            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                ticket = await uow.TicketRepository.GetByIdAsync(id);
            }

            return ticketMapper.Map(ticket);
        }

        public async Task<int> AddTicketAsync(RequestFromUserModel model)
        {
            using(var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                Ticket ticketEntity = ticketMapper.Map(model);
                uow.TicketRepository.Add(ticketEntity);
                return await uow.SaveAsync();
            }
        }

        public async Task<int> UpdateAsync(TicketForListModel ticket)
        {
            using(var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                uow.TicketRepository.Update(ticketMapper.Map(ticket));
                return await uow.SaveAsync();
            }
        }

        public async Task<bool> RemoveAsync(int id)
        {

            using (var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                var ticket = await uow.TicketRepository.GetByIdAsync(id);

                if (ticket == null)
                {
                    return false;
                }

                uow.TicketRepository.Delete(id);
                await uow.SaveAsync();
            }

            return true;
        }

        public async Task<int> GetAmountOfUnreadTicketForAdmin()
        {
            using(var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.TicketRepository.GetAll().Where(ticket => ticket.IsReadByAdmin == false)
                    .CountAsync();
            }
        }

        public async Task<int> GetAmountOfUnreadTicketForUser(int userId)
        {
            using(var uow = unitOfWorkFactory.GetUnitOfWork())
            {
                return await uow.TicketRepository.GetAll().Where(ticket => ticket.OwnerId == userId && ticket.IsReadByUser == false)
                    .CountAsync();
            }
        }
    }
}
