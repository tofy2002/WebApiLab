using Lab3.Models;

namespace Lab3.Repository.interfaces
{
    public interface IDepartmentRepo
    {
        Task<Department?> GetByIdAsync(int id);

        Task<List<Department>> GetAllWithStudentsAsync();

        Task AddAsync(Department department);

        Task<bool> DeleteAsync(int id);
    }
}
