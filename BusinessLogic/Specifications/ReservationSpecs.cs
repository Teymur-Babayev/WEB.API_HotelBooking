using Ardalis.Specification;
using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BusinessLogic.Specifications
{
    internal static class ReservationSpecs
    {
        internal class ByUser : Specification<Reservation>
        {
            public ByUser(string userId)
            {
                Query
                    .Where(x => x.UserId == userId)
                    .Include(x => x.Room);
            }
        }
    }
}
