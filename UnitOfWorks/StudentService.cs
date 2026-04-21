using AutoMapper;
using Lab2.DTOs.StudentDTOS;
using Lab3.Exceptions;
using Lab3.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Lab2.UnitOfWorks
{
    public class StudentService
    {
        private readonly UnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentService> logger;

        public StudentService(UnitOfWork unit, IMapper mapper,ILogger<StudentService> logger)
        {
            _unit = unit;
            _mapper = mapper;
            this.logger = logger;   
        }
        public async Task<object> GetAll(string? search, int page, int pageSize,string? sortby,string? sortDir)
        {
            var query = _unit.StudentRepo
                .GetAllQueryable(s => s.Dept, s => s.StSuperNavigation)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.StFname.Contains(search) ||
                    s.StLname.Contains(search));
            }
            sortby= sortby?.ToLower();
            sortDir= sortDir?.ToLower()== "desc" ? "desc" : "asc";
            query = (sortby, sortDir) switch
            {
                ("fname", "desc") => query.OrderByDescending(s => s.StFname),
                ("fname", _) => query.OrderBy(s => s.StFname),

                ("lname", "desc") => query.OrderByDescending(s => s.StLname),
                ("lname", _) => query.OrderBy(s => s.StLname),

                _ => query.OrderBy(s => s.StId)
            };
            var total = await query.CountAsync();
            var students = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var studentDTOs = _mapper.Map<List<StudentDTO>>(students);

            return new
            {
                data = studentDTOs,
                page,
                pageSize, total
            };
        }
        public async Task<StudentDTO> GetById(int id)
        {
            logger.LogInformation("Fetching student with ID: {Id}", id);
            var student = _unit.StudentRepo.GetAllQueryable(s => s.Dept, s => s.StSuperNavigation).AsNoTracking().FirstOrDefault(s => s.StId == id);
            if (student == null)
            {
                logger.LogError("Student with ID: {Id} not found", id);
                throw new NotFoundException($"No student with this id : {id}");
            }

            return _mapper.Map<StudentDTO>(student);
        }
        public async Task<(bool success, string message, Student stdto)> Add(CreatedStudentDTO DTO)
        {
            var deptExists = await _unit.DepartmentRepo.GetById(DTO.deptID);
            if (deptExists == null)
                throw new BadRequestException($"Department with ID {DTO.deptID} does not exist. Please provide a valid department ID.");
            var exists = await _unit.StudentRepo.GetById(DTO.ID);
            if (exists != null)
                throw new BadRequestException($"Student with ID {DTO.ID} already exists. Please provide a unique student ID.");
            logger.LogInformation(
    "[StudentService] Creating student | Name: {Name} | Age: {Age} | DeptId: {DeptId}",
    DTO.StLname,
    DTO.age,    
    DTO.deptID
);
            var stdto = _mapper.Map<Student>(DTO);
            await _unit.StudentRepo.Add(stdto);

            await _unit.Save();

            return (true, "Student added successfully", stdto);
        }
        public async Task<bool> Update(int id, CreatedStudentDTO DTO)
        {
            var student = await _unit.StudentRepo.GetById(id);

            if (student == null)
                throw new NotFoundException($"No student with this id : {id}");

            _mapper.Map(DTO, student); // now safe (ID ignored)

            await _unit.Save();

            return true;
        }
        public async Task<bool> Delete(int id)
        {
            var deleted = await _unit.StudentRepo.DeleteAsync(id);
            if (!deleted)
                throw new BadRequestException($"No student with this {id} ");
            await _unit.Save();
            return true;

        }
    }
}
