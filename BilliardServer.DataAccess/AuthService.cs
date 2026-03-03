using BilliardServer.Core.Abstractions;
using BilliardServer.Core.Common;
using BilliardServer.Core.Enums;
using BilliardServer.Core.Models;
using BilliardServer.DataAccess.Extensions;
using BilliardServer.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace BilliardServer.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<UserEntity> _userManager; 
        private readonly TokenService _tokenService;

        public AuthService(UserManager<UserEntity> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<User>> RegisterByEmail(string name, string email, string password)
        {
            if (_userManager.FindByEmailAsync(email).Result != null)
                return Result<User>.Fail("Email is already taken");

            var createEntityResult = await CreateUserEntity(name, email, password);

            if (!createEntityResult.IsSuccess)
                return Result<User>.Fail(createEntityResult.Error!);

            return Result<User>.Ok(createEntityResult.Value!.CreateUser());
        }

        public async Task<AuthResult> LoginByEmail(string email, string password)
        {
            var userEntity = await _userManager.FindByEmailAsync(email);
            if (userEntity == null)
                return AuthResult.Failure("Invalid credentials");

            var passwordValid = await _userManager.CheckPasswordAsync(userEntity, password);
            if (!passwordValid)
            {
                await _userManager.AccessFailedAsync(userEntity);
                if (await _userManager.IsLockedOutAsync(userEntity))
                    return AuthResult.Lockout();

                return AuthResult.Failure("Invalid credentials");
            }

            await _userManager.ResetAccessFailedCountAsync(userEntity);

            var token = await _tokenService.GenerateAccessToken(userEntity);

            if (await _userManager.GetTwoFactorEnabledAsync(userEntity))
                return AuthResult.Requires2FA(token);

            return AuthResult.Success(token);
        }

        public async Task<AuthByProviderResult> LoginByProvider(string provider, string providerKey, string name, string? email)
        {
            var userEntity = await _userManager.FindByLoginAsync(provider, providerKey);

            var isFirstAuth = userEntity == null;

            if (userEntity == null)
            {
                var createEntityResult = await CreateUserEntity(name, email);
                if (!createEntityResult.IsSuccess)
                    return AuthByProviderResult.Fail(createEntityResult.Error!);
                
                userEntity = createEntityResult.Value!;
            }

            var tokenData = await _tokenService.GenerateAccessToken(userEntity);

            return AuthByProviderResult.Ok(tokenData, userEntity.CreateUser(), isFirstAuth);
        }

        private async Task<Result<UserEntity>> CreateUserEntity(string name, string? email = null, string? password = null, int avatar = 1)
        {
            var errorsOrNull = User.ValidateParamsForNew(name, avatar);
            if (string.IsNullOrEmpty(errorsOrNull) == false)
                return Result<UserEntity>.Fail(errorsOrNull);

            var userEntity = new UserEntity
            {
                UserName = Guid.NewGuid().ToString("N"),
                Name = name,
                Avatar = avatar,
                Email = email
            };

            var result = !string.IsNullOrEmpty(password)
                ? await _userManager.CreateAsync(userEntity, password)
                : await _userManager.CreateAsync(userEntity);

            if (!result.Succeeded)
                return Result<UserEntity>.Fail(string.Join(",", result.Errors));

            await _userManager.AddToRoleAsync(userEntity, UserRoleType.User);

            return Result<UserEntity>.Ok(userEntity);
        }
    }
}
