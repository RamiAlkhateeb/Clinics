using AutoMapper;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services
{
    public interface ISlotService
    {


        List<Slot> GetSlotsOfUser(int doctorId);

        void Book(Slot slot);

        void Cancel(int id);

        Slot GetSlot(int id);

    }

    public class SlotService : ISlotService
    {
        private DataContext _context;
        private readonly IMapper _mapper;

        public Functions functions;

        public SlotService(
            DataContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            functions = new Functions();
        }





        public void Book(Slot slot)
        {
            // validate
            var user = _context.Users.FirstOrDefault(
                u => u.Id == slot.PatientId &&
                u.Role == Role.Patient);
            if (user == null) throw new KeyNotFoundException("User not found");

            List<Slot> slots = GetSlotsOfUser(slot.DoctorId);


            if (!functions.isDoctorFull(slots))
            {
                // map slot to new Slot object
                var booking = _mapper.Map<Slot>(slot);

                // save user
                _context.Slots.Add(booking);
                _context.SaveChanges();
            }
            else
            {
                throw new KeyNotFoundException("Doctor is full");

            }

        }





        public List<Slot> GetSlotsOfUser(int userId)
        {
            var user = _context.Users.Include(u => u.Slots)
            .FirstOrDefault(u =>u.Id == userId );
            if (user == null) throw new KeyNotFoundException("User not found");

            var slots = user.Slots;

            if (user.Role == Role.Patient)
                slots = _context.Slots.Where(s => s.PatientId == userId).ToList();



            return slots;
        }

        public void Cancel(int id)
        {
            var slot = GetSlot(id);
            _context.Slots.Remove(slot);
            _context.SaveChanges();
        }

        public Slot GetSlot(int id)
        {
            var slot = _context.Slots.FirstOrDefault(s =>
                    s.Id == id
                );
            if (slot == null) throw new KeyNotFoundException("Slot not found");
            return slot;
        }




    }
}