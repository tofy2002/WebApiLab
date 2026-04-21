using System.ComponentModel.DataAnnotations;

namespace Lab2.DTOs.StudentDTOS
{
    public class RegisterDTO
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string fullname { get; set; }
        [Required]
        public int age { get; set; }
    }
}
