using Lab2.Repository;
using Lab3.Models;
using Lab3.Repository;
using Lab3.Repository.interfaces;

namespace Lab2.UnitOfWorks
{
    public class UnitOfWork
    {
        private readonly ITIDbContext _db;

        private StudentRepo _studentRepo;               // FIELD
        private IDepartmentRepo _departmentRepo;

        public UnitOfWork(ITIDbContext db)
        {
            _db = db;
        }

        // ✔ PROPERTY
        public StudentRepo StudentRepo
        {
            get
            {
                if (_studentRepo == null)
                    _studentRepo = new StudentRepo(_db);

                return _studentRepo;
            }
        }

       

        public IDepartmentRepo DepartmentRepo
        {
            get
            {
                if (_departmentRepo == null)
                    _departmentRepo = new DepartmentRepo(_db);

                return _departmentRepo;
            }
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}