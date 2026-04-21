using System.ComponentModel.DataAnnotations;

namespace Lab2.DTOs.StudentDTOS
{
    public class LoginDTO
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}
