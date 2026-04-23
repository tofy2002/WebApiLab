using Lab3.Models;

namespace Lab3.Repository.interfaces
{
    public interface IStudentRepo
    {
        Task<Student?> GetByIdWithDetailsAsync(int id);
        Task<(List<Student>,int)> GetPagedAsync(string? search, int page, int pageSize, string? sortby, string? sortDir);
    }
}
