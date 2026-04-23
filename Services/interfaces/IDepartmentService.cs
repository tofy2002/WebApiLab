using Lab2.DTOs.DepartmentDTOS;
using Lab3.DTOs.DepartmentDTOS;
using Lab3.Models;

namespace Lab3.Services.interfaces
{
    public interface IDepartmentService
    {
        Task<ReadDepartmentDTO?> GetById(int id); 
        Task<List<ReadDepartmentDTO>> GetAll();
        Task<Department?> Add(CreatedDepartmentDTO dto);
        Task<bool> Update(int id, CreatedDepartmentDTO dto);
        Task<bool> Delete(int id);

    }
}
