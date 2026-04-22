using AutoMapper;
using Lab2.DTOs.StudentDTOS;
using Lab3.Exceptions;
using Lab3.Models;
using Lab3.UnitOfWorks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab2.UnitOfWorks
{
    public class StudentService
    {
        private readonly UnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentService> logger;
        //private readonly IMemoryCache _cache;
        private readonly CacheService _cache;

        public StudentService(UnitOfWork unit, IMapper mapper, ILogger<StudentService> logger, CacheService cache)
        {
            _unit = unit;
            _mapper = mapper;
            this.logger = logger;
            _cache = cache;
        }
        public async Task<object> GetAll(string? search, int page, int pageSize,string? sortby,string? sortDir)
        {
            var cacheKey = $"students:" +
               $"search={search ?? "none"}:" +
               $"page={page}:" +
               $"size={pageSize}:" +
               $"sort={sortby ?? "none"}:" +
               $"dir={sortDir ?? "asc"}";
            var cached=await _cache.GetAsync<object>(cacheKey);
            if (cached != null)
            {
                logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return cached;
            }
            logger.LogInformation("CACHE MISS");
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
            //_cache.Set(cacheKey, studentDTOs, TimeSpan.FromSeconds(30));
            var result = new
            {
                data = studentDTOs,
                page,
                pageSize,
                total
            };
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30));
            return result;
        }
        public async Task<StudentDTO> GetById(int id)
        {   string key= $"student:{id}";
           var cachedStudent = await _cache.GetAsync<StudentDTO>(key);
            if (cachedStudent != null)
            {
                logger.LogInformation("Cache hit for student with ID: {Id}", id);
                return cachedStudent;
            }
            logger.LogInformation("CACHE MISS FOR STUDENT ID: {Id}", id);
            logger.LogInformation("Fetching student with ID: {Id}", id);
            var student = _unit.StudentRepo.GetAllQueryable(s => s.Dept, s => s.StSuperNavigation).AsNoTracking().FirstOrDefault(s => s.StId == id);
            if (student == null)
            {
                logger.LogError("Student with ID: {Id} not found", id);
                throw new NotFoundException($"No student with this id : {id}");
            }
            var stdDto=_mapper.Map<StudentDTO>(student);
            await _cache.SetAsync(key, stdDto, TimeSpan.FromSeconds(30));
            return stdDto;
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
            await _cache.RemoveAsync($"student:{DTO.ID}");
            await _cache.RemoveAsync("students");
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
            //_cache.Remove("students");
            _mapper.Map(DTO, student); // now safe (ID ignored)

            await _unit.Save();
            await _cache.RemoveAsync("students");
            await _cache.RemoveAsync($"student:{id}");
            return true;
        }
        public async Task<bool> Delete(int id)
        {
            var deleted = await _unit.StudentRepo.DeleteAsync(id);
            if (!deleted)
                throw new BadRequestException($"No student with this {id} ");
            //_cache.Remove("students");
            await _unit.Save();
            await _cache.RemoveAsync("students");
            await _cache.RemoveAsync($"student:{id}");
            return true;

        }
    }
}
