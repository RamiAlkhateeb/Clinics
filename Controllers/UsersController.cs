using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Users;
using WebApi.Models;
using System.Collections.Generic;
using WebApi.Authorization;
using WebApi.Services;
using WebApi.Entities;
using WebApi.Helpers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.IO;
//using Microsoft.AspNet.OData;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {

        private IUserService _userService;
        private IMapper _mapper;

        public UsersController(
            IUserService userService,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;

        }

        [Authorize(Role.Admin)]
        //[EnableQuery]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok( _userService.GetAll().AsQueryable());
        }

        [HttpGet("csv")]
        [Produces("text/csv")]
        public IActionResult GetCSV()
        {
            var users = _userService.GetAll();

            var builder = new StringBuilder();
            builder.AppendLine("Id,UserName");
            foreach (var user in users)
            {
                builder.AppendLine($"{user.Id},{user.FirstName}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()),"text/csv","users.csv");
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


        [HttpGet]
        //[EnableQuery]
        [Route("doctors")]
        public ActionResult<IQueryable<object>> GetDoctors([FromQuery] FilteringParams filterParameters)
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
