using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using BusinessLogic.Specifications;
using DataAccess.Data;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    internal class ReservationsService : IReservationService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Reservation> reservationR;
        private readonly IRepository<Room> roomR;
        private readonly IEmailSender emailSender;

        public ReservationsService(IMapper mapper,
                            IRepository<Reservation> reservationR,
                            IRepository<Room> roomR,
                            IEmailSender emailSender)
        {
            this.mapper = mapper;
            this.reservationR = reservationR;
            this.roomR = roomR;
            this.emailSender = emailSender;
        }

        public async Task Create(string userId, int id, DateTime date1, DateTime date2)
        {
            var reservation = new Reservation()
            {
                CheckInDate = date1,
                CheckOutDate = date2,
                UserId = userId,
                RoomId = id
            };

            reservationR.Insert(reservation);
            reservationR.Save();
        }

        public async Task<IEnumerable<ReservationDto>> GetAllByUser(string userId)
        {
            var items = await reservationR.GetListBySpec(new ReservationSpecs.ByUser(userId));
            return mapper.Map<IEnumerable<ReservationDto>>(items);
        }
    }
}
