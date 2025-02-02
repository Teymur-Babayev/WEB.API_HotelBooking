using AutoMapper;
using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Repositories;
using BusinessLogic.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogic.Services
{
    public class RoomsService : IRoomsService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Room> roomsR;
        private readonly IRepository<Category> categoriesR;
        //private readonly ShopDbContext context;

        public RoomsService(IMapper mapper,
                                IRepository<Room> roomsR,
                                IRepository<Category> categoriesR)
        {
            this.mapper = mapper;
            this.roomsR = roomsR;
            this.categoriesR = categoriesR;
        }

        public void Create(CreateRoomModel room)
        {
            roomsR.Insert(mapper.Map<Room>(room));
            roomsR.Save();
        }

        public void Delete(int id)
        {
            if (id < 0) throw new HttpException(Errors.IdMustPositive, HttpStatusCode.BadRequest);

            // delete product by id
            var product = roomsR.GetById(id);

            if (product == null) throw new HttpException(Errors.RoomNotFound, HttpStatusCode.NotFound);

            roomsR.Delete(product);
            roomsR.Save();
        }

        public void Edit(RoomDto room)
        {
            roomsR.Update(mapper.Map<Room>(room));
            roomsR.Save();
        }

        public async Task<RoomDto?> Get(int id)
        {
            if (id < 0) throw new HttpException(Errors.IdMustPositive, HttpStatusCode.BadRequest);

            var item = await roomsR.GetItemBySpec(new RoomSpecs.ById(id));
            if (item == null) throw new HttpException(Errors.RoomNotFound, HttpStatusCode.NotFound);

            // load related entity
            //context.Entry(item).Reference(x => x.Category).Load();

            // convert entity type to DTO
            // 1 - using manually (handmade)
            //var dto = new ProductDto()
            //{
            //    Id = product.Id,
            //    CategoryId = product.CategoryId,
            //    Description = product.Description,
            //    Discount = product.Discount,
            //    ImageUrl = product.ImageUrl,
            //    InStock = product.InStock,
            //    Name = product.Name,
            //    Price = product.Price,
            //    CategoryName = product.Category.Name
            //};
            // 2 - using AutoMapper
            var dto = mapper.Map<RoomDto>(item);

            return dto;
        }

        public async Task<IEnumerable<RoomDto>> Get(IEnumerable<int> ids)
        {
            //return mapper.Map<List<ProductDto>>(context.Products
            //    .Include(x => x.Category)
            //    .Where(x => ids.Contains(x.Id))
            //    .ToList());
            return mapper.Map<List<RoomDto>>(await roomsR.GetListBySpec(new RoomSpecs.ByIds(ids)));
        }

        public async Task<IEnumerable<RoomDto>> GetAll()
        {
            return mapper.Map<List<RoomDto>>(await roomsR.GetListBySpec(new RoomSpecs.All()));
        }

        public IEnumerable<CategoryDto> GetAllCategories()
        {
            return mapper.Map<List<CategoryDto>>(categoriesR.GetAll());
        }
    }
}
