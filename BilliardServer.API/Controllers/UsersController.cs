using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Dto.Web;
using BilliardServer.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilliardServer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [Authorize(Roles = UserRoleType.User)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetUser(string id)
        {
            var result = await _usersService.GetUser(id);

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
