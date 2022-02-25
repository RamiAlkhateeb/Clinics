using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models.Users;
using WebApi.Services;
using WebApi.Entities;
using System.Collections.Generic;
using WebApi.Helpers;


namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SlotsController : ControllerBase
    {
        private ISlotService _slotService;
        private IMapper _mapper;

        public SlotsController(
            ISlotService slotService,
            IMapper mapper)
        {
            _slotService = slotService;
            _mapper = mapper;
        }



        [HttpGet("doctors/{docId}/slots")]
        public IActionResult GetSlots(int docId)
        {
            var slots = _slotService.GetSlotsOfUser(docId);
            List<object> Result = new List<object>();
            Functions func = new Functions();
            bool isFull = func.isDoctorFull(slots);
            if (isFull)
                return Ok("Doctor is Full");
            else
            {
                foreach (var item in slots)
                {
                    var doctor = new
                    {
                        item.Id,
                        item.DoctorId,
                        item.PatientId,
                        item.Duration,
                        item.Number
                    };
                    Result.Add(doctor);
                }
                return Ok(Result);
            }

        }


        /// <summary>
        /// Book an appointment.
        /// </summary>
        [HttpPost]
        [Route("book")]
        public IActionResult Book(Slot slot)
        {
            _slotService.Book(slot);
            return Ok(new { message = "Book created" });
        }



        [HttpDelete("{id}")]
        public IActionResult Cancel(int id)
        {
            _slotService.Cancel(id);
            return Ok(new { message = "Appointment Canceled" });
        }

        [HttpGet("{id}/slotinfo")]
        public IActionResult GetSlot(int id)
        {
            var slot = _slotService.GetSlot(id);
            return Ok(slot);
        }
    }
}
