using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldOfWords.API.Models.Models;
using WorldOfWords.Domain.Models;

namespace WorldOfWords.API.Models.IMappers
{
    public interface ITicketMapper
    {
        Ticket Map(RequestFromUserModel request);
        TicketForListModel Map(Ticket ticket);
        Ticket Map(TicketForListModel ticket);
    }
}
