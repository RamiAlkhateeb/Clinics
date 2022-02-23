using AutoMapper;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using System;
using WebApi.Models.Users;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;


namespace WebApi.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();

        IEnumerable<User> GetAllDoctors(FilteringParams filteringParams);

        User GetById(int id);
        User Login(LoginRequest user);

        void Create(CreateRequest model);


        void Delete(int id);
    }

    public class UserService : IUserService
    {
        private DataContext _context;
        private readonly IMapper _mapper;

        public SlotService slotService;
        public Functions functions;
        public UserService(
            DataContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            functions = new Functions();
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public IEnumerable<User> GetAllDoctors(FilteringParams filteringParams)
        {
            var sortBy = filteringParams.SortBy;
            var filterString = filteringParams.FilterString;
            var doctors = _context.Users.Include(d => d.Slots)
            .Where(d => d.Role == Role.Doctor).ToList();

            var sortedDoctors = functions.SortDoctors(sortBy,doctors);

            if (!String.IsNullOrEmpty(filterString))
            {
                filterString = filterString.ToLower();
                if (filterString == "mostappointments")
                {
                    var mostVisitedDoctor = sortedDoctors.FirstOrDefault();
                    sortedDoctors = new List<User>();
                    sortedDoctors.Add(mostVisitedDoctor);
                }
                if(filterString == "morethan6h")
                {
                    var mostVisitedDoctors = new List<User>();
                    foreach (var item in sortedDoctors)
                    {
                        if(functions.HoursCount(item.Slots) > 360)
                            mostVisitedDoctors.Add(item);
                        
                    }
                    sortedDoctors = mostVisitedDoctors;
                    
                }
            }

          
            return sortedDoctors;
        }

        public User GetById(int id)
        {
            return getUser(id);
        }

        public void Create(CreateRequest model)
        {
            // validate
            if (_context.Users.Any(x => x.Email == model.Email))
                throw new AppException("User with the email '" + model.Email + "' already exists");

            // map model to new user object
            var user = _mapper.Map<User>(model);

            // hash password
            user.PasswordHash = BCryptNet.HashPassword(model.Password);

            // save user
            _context.Users.Add(user);
            _context.SaveChanges();
        }


        
        public void Delete(int id)
        {
            var user = getUser(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        // helper methods

        private User getUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Id == id &&
                u.Role == Role.Doctor
            );
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }





        public User Login(LoginRequest loginInfo)
        {
            var user = _context.Users.FirstOrDefault(user => user.Email.Equals(loginInfo.Email));
            if (user == null) throw new KeyNotFoundException("User not found");
            if (!BCryptNet.Verify(loginInfo.Password, user.PasswordHash))
                throw new System.Exception("Wrong Password");
            return user;
        }
    }
}