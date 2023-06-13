using System.Security.Claims;
using api.Data;
using api.DTOs;
using api.Entities;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHashingService _passwordHashingService;

        public AccountController(DataContext dataContext, ITokenService tokenService, IPasswordHashingService passwordHashingService)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _passwordHashingService = passwordHashingService;
        }


        [HttpPost("register")]
        public async Task<dynamic> Register([FromBody] RegisterDto payload)
        {
            if (await UserExists(username: payload.UserName))
                return BadRequest(new BaseResponseDto<string>()
                {
                    message = "Username is taken",
                    statusCode = 400
                });

            var password = _passwordHashingService.HashPassword(password: payload.Password);

            var user = new User()
            {
                UserName = payload.UserName,
                PasswordHash = password.PasswordHash,
                PasswordSalt = password.PasswordSalt
            };

            _dataContext.UserEntity.Add(user);
            await _dataContext.SaveChangesAsync();

            return new BaseResponseDto<User>()
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
                return Unauthorized(new BaseResponseDto<string>()
                {
                    message = "Invalid username or password",
                    statusCode = 401
                });

            if (!_passwordHashingService.ComparePassword(passwordHash: user.PasswordHash, passwordSalt: user.PasswordSalt,
                    password: payload.Password))
                return Unauthorized(new BaseResponseDto<string>()
                {
                    message = "Invalid username or password",
                    statusCode = 401,
                    data = null
                });

            return new BaseResponseDto<UserDto>()
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

            return new BaseResponseDto<UserDto>()
            {
                message = "User retrieved successfully",
                data = new UserDto()
                {
                    Username = user?.UserName
                },
                statusCode = 200
            };
        }
        
        

        private async Task<bool> UserExists(string username)
        {
            return await _dataContext.UserEntity.AnyAsync(user => user.UserName == username);
        }
    }
}