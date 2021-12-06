using System.ComponentModel.DataAnnotations;

namespace scabackend.Models
{
    public class UserLoginModel
    {
        [Required]
        public string account { get; set; }

        [Required]
        public string password { get; set; }
    }
}
