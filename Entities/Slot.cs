using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities
{
    public class Slot
    {
        public int Id { get; set; }
        public int PatientId { get; set; }

        [Range(15, 120)]
        public int Duration { get; set; }
        public int DoctorId { get; set; }

        public User Doctor { get; set; }

        public int Number {get;set;}

        


    }
}