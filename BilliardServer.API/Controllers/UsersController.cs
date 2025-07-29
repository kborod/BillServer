using BilliardServer.API.DTOs;
using BilliardServer.Core.Abstractions;
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

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            var users = await _usersService.GetAll();
            var response = users.Select(u => new UserResponse(u.Id, u.Name, u.Avatar));

            return Ok(users);
        }
    }
}
