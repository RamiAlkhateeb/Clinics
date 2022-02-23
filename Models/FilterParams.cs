using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

namespace WebApi.Models
{
    public class FilteringParams
    {
      

        public string SortBy { get; set; } 

        public string FilterString {get; set;} 

        
    }
}