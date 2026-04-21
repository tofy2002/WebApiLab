using Lab3.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ITIDbContext : IdentityDbContext<ApplicationUser>
{
    public ITIDbContext(DbContextOptions<ITIDbContext> options)
        : base(options)
    {
    }

    // ================= Tables =================
    public DbSet<Course> Courses { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<InsCourse> InsCourses { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<StudCourse> StudCourses { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ================= Course =================
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CrsId);
            entity.ToTable("Course");

            entity.Property(e => e.CrsId).HasColumnName("Crs_Id");
            entity.Property(e => e.CrsName).HasColumnName("Crs_Name");
            entity.Property(e => e.CrsDuration).HasColumnName("Crs_Duration");
            entity.Property(e => e.TopId).HasColumnName("Top_Id");
        });

        // ================= Department =================
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptId);
            entity.ToTable("Department");

            entity.Property(e => e.DeptId).HasColumnName("Dept_Id");
            entity.Property(e => e.DeptName).HasColumnName("Dept_Name");
            entity.Property(e => e.DeptDesc).HasColumnName("Dept_Desc");
            entity.Property(e => e.DeptLocation).HasColumnName("Dept_Location");
            entity.Property(e => e.DeptManager).HasColumnName("Dept_Manager");
            entity.Property(e => e.ManagerHiredate).HasColumnName("Manager_Hiredate");

            // ❌ ignore problematic navigation
            entity.Ignore(d => d.DeptManagerNavigation);
        });

        // ================= Instructor =================
        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InsId);
            entity.ToTable("Instructor");

            entity.Property(e => e.InsId).HasColumnName("Ins_Id");
            entity.Property(e => e.InsName).HasColumnName("Ins_Name");
            entity.Property(e => e.InsDegree).HasColumnName("Ins_Degree");
            entity.Property(e => e.DeptId).HasColumnName("Dept_Id");
        });

        // ================= Department → Instructors =================
        modelBuilder.Entity<Department>()
            .HasMany(d => d.Instructors)
            .WithOne(i => i.Dept)
            .HasForeignKey(i => i.DeptId)
            .OnDelete(DeleteBehavior.NoAction);

        // ================= Department → Students =================
        modelBuilder.Entity<Department>()
            .HasMany(d => d.Students)
            .WithOne(s => s.Dept)
            .HasForeignKey(s => s.DeptId)
            .OnDelete(DeleteBehavior.NoAction);

        // ================= Topic → Courses =================
        modelBuilder.Entity<Topic>()
            .HasMany(t => t.Courses)
            .WithOne(c => c.Top)
            .HasForeignKey(c => c.TopId)
            .OnDelete(DeleteBehavior.NoAction);

        // ================= InsCourse (MANY-TO-MANY) =================
        modelBuilder.Entity<InsCourse>(entity =>
        {
            entity.HasKey(e => new { e.InsId, e.CrsId });

            entity.ToTable("Ins_Course");

            entity.Property(e => e.InsId).HasColumnName("Ins_Id");
            entity.Property(e => e.CrsId).HasColumnName("Crs_Id");

            entity.HasOne(e => e.Ins)
                .WithMany(i => i.InsCourses)
                .HasForeignKey(e => e.InsId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.Crs)
                .WithMany(c => c.InsCourses)
                .HasForeignKey(e => e.CrsId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // ================= StudCourse (MANY-TO-MANY) =================
        modelBuilder.Entity<StudCourse>(entity =>
        {
            entity.HasKey(e => new { e.CrsId, e.StId });

            entity.ToTable("Stud_Course");

            entity.Property(e => e.CrsId).HasColumnName("Crs_Id");
            entity.Property(e => e.StId).HasColumnName("St_Id");

            entity.HasOne(e => e.Crs)
                .WithMany(c => c.StudCourses)
                .HasForeignKey(e => e.CrsId);

            entity.HasOne(e => e.St)
                .WithMany(s => s.StudCourses)
                .HasForeignKey(e => e.StId);
        });

        // ================= Student =================
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StId);
            entity.ToTable("Student");

            entity.Property(e => e.StId).HasColumnName("St_Id");
            entity.Property(e => e.StFname).HasColumnName("St_Fname");
            entity.Property(e => e.StLname).HasColumnName("St_Lname");
            entity.Property(e => e.StAddress).HasColumnName("St_Address");
            entity.Property(e => e.StAge).HasColumnName("St_Age");
            entity.Property(e => e.DeptId).HasColumnName("Dept_Id");
            entity.Property(e => e.StSuper).HasColumnName("St_Super");
        });

        // ================= Student Self-Reference =================
        modelBuilder.Entity<Student>()
            .HasOne(s => s.StSuperNavigation)
            .WithMany(s => s.InverseStSuperNavigation)
            .HasForeignKey(s => s.StSuper)
            .OnDelete(DeleteBehavior.NoAction);

        // ================= Topic =================
        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.TopId);
            entity.ToTable("Topic");

            entity.Property(e => e.TopId).HasColumnName("Top_Id");
            entity.Property(e => e.TopName).HasColumnName("Top_Name");
        });

        // ================= Sale (NO PRIMARY KEY) =================
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasNoKey();
            entity.ToTable("sales");
        });
    }
}