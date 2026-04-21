namespace Lab2.DTOs.DepartmentDTOS
{
    public class CreatedDepartmentDTO
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; } = " ";
        public string? DeptDesc { get; set; } 
        public string? DeptLocation { get; set; }
        public int? DeptManager { get; set; } = null;
    }
}
