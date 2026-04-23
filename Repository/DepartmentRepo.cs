using Lab2.Repository;
using Lab3.Models;
using Lab3.Repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Repository
{
    public class DepartmentRepo : GenericRepo<Department>, IDepartmentRepo
    {
        private readonly ITIDbContext _context;

        public DepartmentRepo(ITIDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<List<Department>> GetAllWithStudentsAsync()
        {
            return await _context.Departments
                .Include(d => d.Students)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return false;

            _context.Departments.Remove(dept);
            return true;
        }
    }

   
}   