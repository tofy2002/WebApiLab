using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Lab2.DTOs.StudentDTOS
{
    public class CreatedStudentDTO
    {

        public int ID { get; set; }
       
        public string StFname { get; set; }
        
        public string StLname { get; set; }
        
        public int age { get; set; }
      
        public int deptID { get; set; }
    }
}
