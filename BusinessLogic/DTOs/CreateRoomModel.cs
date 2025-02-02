using DataAccess.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs
{
    public class CreateRoomModel
    {
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public IFormFile Image { get; set; }
    }
}