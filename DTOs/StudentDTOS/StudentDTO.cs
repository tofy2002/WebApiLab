using System.ComponentModel.DataAnnotations;

namespace Lab2.DTOs.StudentDTOS
{
    public class StudentDTO
    {
        public int StId { get; set; }
        public string FullName { get; set; } = " ";

        public int? Age { get; set; }

        public string? DepartmentName { get; set; }

        public string? SupervisorName { get; set; }
    }
}
