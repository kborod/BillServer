using BilliardServer.Application.Features.Users;
using BilliardServer.Core.Common.Dto.Auth;
using BilliardServer.Infrastructure.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BilliardServer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly ILogger _logger;

        public AuthController(IMediator mediator, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, ILogger logger)
        {
            _mediator = mediator;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var registerResult = await _mediator.Send(new RegisterByEmailCommand(dto.Name, dto.Email, dto.Password));

            if (!registerResult.IsSuccess)
                return BadRequest(registerResult.Error);

            return Ok("Registration complete");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var loginResult = await _mediator.Send(new LoginByEmailCommand(dto.Email, dto.Password));

            if (!loginResult.IsSuccess)
            {
                if (loginResult.IsLockedOut) return Unauthorized("Locked");
                if (loginResult.RequiresTwoFactor) return Ok(new { requires2fa = true });
                return BadRequest(loginResult.Error);
            }
            return Ok(new LoginResponseDto { TokenData = loginResult.TokenData });
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            //var loginResult = await _sender.Send(new LoginByEmailCommand(dto.Email, dto.Password));

            //if (!loginResult.IsSuccess)
            //{
            //    if (loginResult.IsLockedOut) return Unauthorized("Locked");
            //    if (loginResult.RequiresTwoFactor) return Ok(new { requires2fa = true });
            //    return BadRequest(loginResult.Error);
            //}

            //return (new LoginResponseDto { TokenData = loginResult.TokenData });

            return BadRequest("Not implemented");
        }

        [AllowAnonymous]
        [HttpGet("Externallogin")]
        public async Task<IActionResult> Externallogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalloginCallback", "Auth");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        [HttpGet("ExternalloginCallback")]
        public async Task<IActionResult> ExternalloginCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return BadRequest("Error loading external login information");

            var email = GetEmailFromExternalProvider(info);
            var name = GetNameFromExternalProvider(info);

            var loginResult = await _mediator.Send(new LoginByProviderCommand(info.LoginProvider, info.ProviderKey, name, email));
            
            if (!loginResult.IsSuccess)
                return BadRequest(loginResult.Error);

            if (loginResult.IsFirstAuth)
            {
                var userEntity = await _userManager.FindByIdAsync(loginResult.User!.Id.ToString());
                var result = await _userManager.AddLoginAsync(userEntity!, info);
                if (!result.Succeeded)
                {
                    _ = _userManager.DeleteAsync(userEntity!);
                    _logger.LogError($"Error associating external login: {string.Join(", ", result.Errors)}");
                    return BadRequest("Error associating external login");
                }
            }

            return Ok(new { accessToken = loginResult.Token });
        }

        private string GetNameFromExternalProvider(ExternalLoginInfo info)
        {
            string result = string.Empty;

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                result = info.Principal.FindFirstValue(ClaimTypes.GivenName)!;

            if (string.IsNullOrEmpty(result))
            {
                Random rnd = new Random();
                result = $"{info.LoginProvider}_{rnd.Next(100000, 999999)}".ToLower();
            }
            return result;
        }

        private string? GetEmailFromExternalProvider(ExternalLoginInfo info)
        {
            string? result = null;
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                result = info.Principal.FindFirstValue(ClaimTypes.Email)!;
            return result;
        }
    }
}
