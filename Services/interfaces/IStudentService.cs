using Lab2.DTOs.StudentDTOS;
using Lab3.DTOs;
using Lab3.Models;

namespace Lab3.Services.interfaces
{
    public interface IStudentService
    {
        Task<PagedResponse<StudentDTO>> GetAll(string? search, int page, int pageSize, string? sortby, string? sortDir);

        Task<StudentDTO> GetById(int id);

        Task<StudentDTO> Add(CreatedStudentDTO DTO);

        Task Update(int id, CreatedStudentDTO DTO);

        Task Delete(int id);
    }
}
