using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    internal interface IReservationService
    {
        Task<IEnumerable<ReservationDto>> GetAllByUser(string userId);
        Task Create(string userId, int id, DateTime date1, DateTime date2);
    }
}
