using BusinessLogic.DTOs;
using BusinessLogic.Interfaces;
using HotelProject.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomsService roomsService;

        public RoomsController(IRoomsService roomsService)
        {
            this.roomsService = roomsService;
        }

        [HttpGet("all")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.ADULT)]
        public async Task<IActionResult> Get()
        {
            return Ok(await roomsService.GetAll());
        }

        //[Authorize] // based on cookies
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // based on JWT
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            return Ok(await roomsService.Get(id));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = Policies.PREMIUM_CLIENT)]
        public IActionResult Create([FromForm] CreateRoomModel model)
        {
            roomsService.Create(model);
            return Ok();
        }


        [HttpPut]
        public IActionResult Edit([FromBody] RoomDto model)
        {
            roomsService.Edit(model);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.ADMIN)]
        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            roomsService.Delete(id);
            return Ok();
        }
    }
}
