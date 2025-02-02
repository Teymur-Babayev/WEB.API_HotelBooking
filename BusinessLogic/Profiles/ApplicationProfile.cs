using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogic.Profiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile(IFileService fileService)
        {
            CreateMap<RoomDto, Room>()
                .ForMember(x => x.Category, opt => opt.Ignore());
            CreateMap<Room, RoomDto>();

            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Reservation, ReservationDto>().ReverseMap();

            CreateMap<CreateRoomModel, Room>()
                .ForMember(x => x.ImageUrl, opt => opt.MapFrom(src => fileService.SaveProductImage(src.Image).Result));

            CreateMap<RegisterModel, User>()
                .ForMember(x => x.UserName, opts => opts.MapFrom(s => s.Email));
        }
    }
}
