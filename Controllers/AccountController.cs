using System.Security.Claims;
using api.Data;
using api.DTOs;
using api.Entities;
using api.Interfaces;
using api.Shared;
using api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext dataContext, ITokenService tokenService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
        }


        [HttpPost("register")]
        public async Task<dynamic> Register([FromBody] RegisterDto payload)
        {
            if (await UserExists(username: payload.UserName))
                return BadRequest(new BaseResponse<string>()
                {
                    message = "Username is taken",
                    statusCode = 400
                });

            var password = PasswordService.HashPassword(password: payload.Password);

            var user = new User()
            {
                UserName = payload.UserName,
                PasswordHash = password.PasswordHash,
                PasswordSalt = password.PasswordSalt
            };

            _dataContext.UserEntity.Add(user);
            await _dataContext.SaveChangesAsync();

            return new BaseResponse<User>()
            {
                message = "User created successfully",
                data = user,
                statusCode = 201
            };
        }

        [HttpPost("login")]
        public async Task<dynamic> Login([FromBody] LoginDto payload)
        {
            var user = await _dataContext.UserEntity.FirstOrDefaultAsync(user => user.UserName == payload.UserName);

            if (user == null)
                return Unauthorized(new BaseResponse<string>()
                {
                    message = "Invalid username or password",
                    statusCode = 401
                });

            if (!PasswordService.ComparePassword(passwordHash: user.PasswordHash, passwordSalt: user.PasswordSalt,
                    password: payload.Password))
                return Unauthorized(new BaseResponse<string>()
                {
                    message = "Invalid username or password",
                    statusCode = 401,
                    data = null
                });

            return new BaseResponse<UserDto>()
            {
                message = "Login successful",
                data = new UserDto()
                {
                    Username = user.UserName,
                    AccessToken = _tokenService.CreateToken(user: user)
                },
                statusCode = 200
            };
        }

        [Authorize]
        [HttpPost("me")]
        public async Task<dynamic> Me()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _dataContext.UserEntity.FirstOrDefaultAsync(user => user.UserName == username);

            return new BaseResponse<User>()
            {
                message = "User retrieved successfully",
                data = user,
                statusCode = 200
            };
        }
        
        

        private async Task<bool> UserExists(string username)
        {
            return await _dataContext.UserEntity.AnyAsync(user => user.UserName == username);
        }
    }
}