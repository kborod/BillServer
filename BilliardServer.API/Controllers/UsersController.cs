using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Enums;
using BilliardServer.Core.Models;
using Kborod.SharedDto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilliardServer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMediator _mediator;

        public UsersController(IUsersService usersService, IMediator mediator)
        {
            _usersService = usersService;
            _mediator = mediator;
        }

        //[Authorize(Roles = UserRoleType.User)]
        [HttpGet("GetUser")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var result = await _usersService.GetUser(id);

            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest(result.Error);
        }

        //[Authorize(Roles = UserRoleType.User)]
        [HttpGet("GetProfile")]
        public async Task<ActionResult<UserProfile>> GetProfile(string id)
        {
            var result = await _mediator.Send(new GetUserProfileCommand(id));

            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest(result.Error);
        }

        //[Authorize(Roles = UserRoleType.User)]
        [HttpGet("GetProfiles")]
        public async Task<ActionResult<int[]>> GetUserProfile([FromQuery] List<string> ids)
        {
            var result = await _mediator.Send(new GetUserProfilesCommand(ids));

            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return BadRequest(result.Error);
        }

        //[Authorize(Roles = RoleType.User)]
        //[HttpPost]
        //public async Task<ActionResult<UserResponse>> AddUser(string name, int avatar)
        //{
        //    var user = await _usersService.Create(name, avatar);

        //    return Ok(new UserResponse(user.Id, user.Name, user.Avatar));
        //}
    }
}
