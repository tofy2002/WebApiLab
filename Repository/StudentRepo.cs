using Lab2.Repository;
using Lab3.Models;
using Lab3.Repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Repository
{
    public class StudentRepo : GenericRepo<Student>,IStudentRepo
    {
        private readonly ITIDbContext _context;
        public StudentRepo(ITIDbContext context): base(context)
        {
            _context = context;
        }
        public async Task<Student?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Students.Include(s => s.Dept).Include(s => s.StSuperNavigation).AsNoTracking()
                .FirstOrDefaultAsync(s => s.StId == id);
        }
        public async Task<(List<Student>, int)> GetPagedAsync(string? search, int page, int pageSize, string? sortby, string? sortDir)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 5;
            var query = _context.Students.Include(s => s.Dept).Include(s => s.StSuperNavigation).AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.StFname!.Contains(search) ||
                    s.StLname!.Contains(search));
            }

            query = (sortby, sortDir) switch
            {
                ("fname", "desc") => query.OrderByDescending(s => s.StFname),
                ("fname", _) => query.OrderBy(s => s.StFname),
                ("sname", "desc") => query.OrderByDescending(s => s.StFname),
                _ => query.OrderBy(s => s.StId)
            };

            var total = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, total);
        }
    }
}
