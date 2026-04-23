using AutoMapper;
using Lab2.DTOs.StudentDTOS;
using Lab2.UnitOfWorks;
using Lab3.DTOs;
using Lab3.Exceptions;
using Lab3.Models;
using Lab3.Services.interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab3.Services
{
    public class StudentService:IStudentService
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
        public async Task<PagedResponse<StudentDTO>> GetAll(string? search, int page, int pageSize,string? sortby,string? sortDir)
        {
            var cacheKey = $"students:" +$"search={search ?? "none"}:" + $"page={page}:" + $"size={pageSize}:" +$"sort={sortby ?? "none"}:" +$"dir={sortDir ?? "asc"}";
            var cached=await _cache.GetAsync<PagedResponse<StudentDTO>>(cacheKey);
            if (cached != null)
            {
                logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return cached;
            }
            logger.LogInformation("CACHE MISS");
            var (students, total) = await _unit.StudentRepo.GetPagedAsync(search, page, pageSize, sortby, sortDir);
            var studentDTOs = _mapper.Map<List<StudentDTO>>(students);
            var result = new PagedResponse<StudentDTO>()
            {
                Data = studentDTOs,
                Page=page,
                PageSize=pageSize,
                TotalCount = total
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
            var student =  await _unit.StudentRepo.GetByIdWithDetailsAsync(id);
            if (student == null)
            {
                logger.LogError("Student with ID: {Id} not found", id);
                throw new NotFoundException($"No student with this id : {id}");
            }
            var stdDto=_mapper.Map<StudentDTO>(student);
            await _cache.SetAsync(key, stdDto, TimeSpan.FromSeconds(30));
            return stdDto;
        }
        public async Task<StudentDTO> Add(CreatedStudentDTO DTO)
        {
            var deptExists = await _unit.DepartmentRepo.GetByIdAsync(DTO.deptID);
            if (deptExists == null)
                throw new BadRequestException($"Department with ID {DTO.deptID} does not exist. Please provide a valid department ID.");
            logger.LogInformation("[StudentService] Creating student | Name: {Name} | Age: {Age} | DeptId: {DeptId}", DTO.StLname, DTO.age, DTO.deptID);
            await _cache.RemoveAsync($"student:{DTO.ID}");
            await _cache.RemoveAsync("students");
            var stdto = _mapper.Map<Student>(DTO);
            await _unit.StudentRepo.Add(stdto);
            await _unit.Save();

            return _mapper.Map<StudentDTO>(stdto);
        }
        public async Task Update(int id, CreatedStudentDTO DTO)
        {
            var student = await _unit.StudentRepo.GetById(id);

            if (student == null)
                throw new NotFoundException($"No student with this id : {id}");
            _mapper.Map(DTO, student); // now safe (ID ignored)

            await _unit.Save();
            await _cache.RemoveAsync("students");
            await _cache.RemoveAsync($"student:{id}");
            
        }
        public async Task Delete(int id)
        {
            var deleted = await _unit.StudentRepo.DeleteAsync(id);
            if (!deleted)
                throw new BadRequestException($"No student with this {id} ");
            //_cache.Remove("students");
            await _unit.Save();
            await _cache.RemoveAsync("students");
            await _cache.RemoveAsync($"student:{id}");
           

        }
    }
}
