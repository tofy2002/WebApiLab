using Lab3.Models;
using Lab2.Repository;


namespace Lab2.UnitOfWorks
{
    public class UnitOfWork
    {
        ITIDbContext db;
        GenericRepo<Student> stdrepo;
        GenericRepo<Department> deptrepo;
        public UnitOfWork(ITIDbContext db)
        {
            this.db = db;
        }
        public GenericRepo<Student> StudentRepo
        {
            get
            {
                if (stdrepo == null)
                    stdrepo = new GenericRepo<Student>(db);
                return stdrepo;
            }
        }
        public GenericRepo<Department> DepartmentRepo
        {
            get
            {
                if (deptrepo == null)
                    deptrepo = new GenericRepo<Department>(db);
                return deptrepo;
            }
        }
        public async Task Save()
        {
           await db.SaveChangesAsync();
        }

    }
}
