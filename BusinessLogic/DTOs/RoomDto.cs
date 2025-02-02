using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs
{
    public class RoomDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<ReservationDto>? Reservations { get; set; }
    }
}
