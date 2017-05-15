using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.API.Models.IMappers;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.Mappers
{
    public class TicketMapper : ITicketMapper
    {
        public Ticket Map(RequestFromUserModel request)
        {
            return new Ticket()
            {
                Subject = request.Subject,
                Description = request.Description,
                OwnerId = request.OwnerId,
                OpenDate = DateTime.Now,
                GroupId = request.GroupId,
                IsReadByUser = request.IsReadByUser,
                IsReadByAdmin = request.IsReadByAdmin
            };
        }

        public TicketForListModel Map(Ticket ticket)
        {
            return new TicketForListModel
            {
                TicketId = ticket.TicketId,
                Subject = ticket.Subject,
                Description = ticket.Description,
                OpenDate = ticket.OpenDate,
                CloseDate = ticket.CloseDate,
                Comment = ticket.Comment,
                ReviewStatus = (TicketReviewStatus)ticket.ReviewStatus,
                OwnerId = ticket.OwnerId,
                UserEmail = ticket.User.Email,
                UserName = ticket.User.Name,
                GroupId = ticket.GroupId,
                IsReadByUser = ticket.IsReadByUser,
                IsReadByAdmin = ticket.IsReadByAdmin
            };
        }

        public Ticket Map(TicketForListModel ticket)
        {
            return new Ticket
            {
                TicketId = ticket.TicketId,
                Subject = ticket.Subject,
                Description = ticket.Description,
                OpenDate = ticket.OpenDate,
                CloseDate = ticket.CloseDate,
                Comment = ticket.Comment,
                OwnerId = ticket.OwnerId,
                ReviewStatus = (int)ticket.ReviewStatus,
                GroupId = ticket.GroupId,
                IsReadByUser = ticket.IsReadByUser,
                IsReadByAdmin = ticket.IsReadByAdmin
            };
        }
    }
}
