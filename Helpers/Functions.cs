using System;
using System.Globalization;
using WebApi.Entities;
using System.Collections.Generic;
using System.Linq;



namespace WebApi.Helpers
{
    public class Functions
    {        
        public bool isDoctorFull(List<Slot> slots)
        {
            if(slots.Count() > 11)
                return true;
           
            int hours= HoursCount(slots);

            if (hours > 465)
                return true;
            else
                return false;


        }

        public int HoursCount(List<Slot> slots)
        {
            int hours = 0;
            foreach (var s in slots)
            {
                hours += s.Duration;
            }

            return hours;
        }

        public List<User> SortDoctors(string sortBy, List<User> doctors)
        {
            switch (sortBy)
            {
                case "names":
                    doctors =
                        doctors
                            .OrderBy(d => d.FirstName)
                            .ToList();
                    break;

                default:
                    doctors =
                        doctors
                            .OrderByDescending(d => d.Slots.Count())
                            .ToList();
                    break;
            }
            return doctors;


        }
    }


}