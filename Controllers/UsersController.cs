using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Users;
using WebApi.Models;
using System.Collections.Generic;

using WebApi.Services;
using WebApi.Helpers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private IUserService _userService;
        private IMapper _mapper;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;

        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("doctors/{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            return Ok(user);
        }


        [HttpPost]
        [Route("register")]
        public IActionResult Create(CreateRequest model)
        {
            _userService.Create(model);
            return Ok(new { message = "User created" });
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginRequest model)
        {
            var user = _userService.Login(model);
            
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });


            return Ok(user);
        }

        // private string GenerateJwtToken(string username, List<string> roles)
        // {
        //     var claims = new List<Claim>
        // {
        //     new Claim(JwtRegisteredClaimNames.Sub, username),
        //     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //     new Claim(ClaimTypes.NameIdentifier, username)
        // };

        //     roles.ForEach(role =>
        //     {
        //         claims.Add(new Claim(ClaimTypes.Role, role));
        //     });

        //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]));
        //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //     var token = new JwtSecurityToken(
        //         _configuration["JwtIssuer"],
        //         _configuration["JwtIssuer"],
        //         claims,
        //         signingCredentials: creds
        //     );

        //     return new JwtSecurityTokenHandler().WriteToken(token);
        // }

        [HttpGet]
        [Route("doctors")]
        public IActionResult GetDoctors([FromQuery] FilteringParams filterParameters)
        {
            var users = _userService.GetAllDoctors(filterParameters);
            List<object> Result = new List<object>();
            Functions func = new Functions();
            foreach (var item in users)
            {
                bool isFull = func.isDoctorFull(item.Slots);
                var doctor = new
                {
                    Id = item.Id,
                    Name = item.FirstName + " " + item.LastName,
                    Email = item.Email,
                    isAvailable = !isFull
                };
                Result.Add(doctor);
            }
            return Ok(Result);
        }


        // [HttpPut("{id}")]
        // public IActionResult Update(int id, UpdateRequest model)
        // {
        //     _userService.Update(id, model);
        //     return Ok(new { message = "User updated" });
        // }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok(new { message = "User deleted" });
        }
    }

    public class LoginResponse
    {
        public string Access_Token;
        public string UserName;
    }
}
