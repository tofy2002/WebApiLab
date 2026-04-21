using AutoMapper;
using Lab2.DTOs.DepartmentDTOS;
using Lab2.UnitOfWorks;
using Lab3.DTOs.DepartmentDTOS;
using Lab3.Models;
using Lab3.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Repository
{
    public class DepartmentService
    {
        private readonly UnitOfWork _unit;
        private readonly IMapper _mapper;

        public DepartmentService(UnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }
        public async Task<ReadDepartmentDTO?> GetById(int id)
        {
            var dept = await _unit.DepartmentRepo.GetById(id);

            if (dept == null)
                return null;

            return _mapper.Map<ReadDepartmentDTO>(dept);
        }

        public async Task<List<ReadDepartmentDTO>> GetAll()
        {
            var departments = await _unit.DepartmentRepo
                .GetAllQueryable(d => d.Students)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<ReadDepartmentDTO>>(departments);
        }

      
        public async Task<Department> Add(CreatedDepartmentDTO dto)
        {
            var department = _mapper.Map<Department>(dto);

            await _unit.DepartmentRepo.Add(department);
            await _unit.Save();

            return department;
        }

      
        public async Task<bool> Update(int id, CreatedDepartmentDTO dto)
        {
            var department = await _unit.DepartmentRepo.GetById(id);
            if (department == null)
                return false;

           
            _mapper.Map(dto, department);

            await _unit.Save();
            return true;
        }

        
        public async Task<bool> Delete(int id)
        {
            var deleted = await _unit.DepartmentRepo.DeleteAsync(id);
            if (!deleted) return false;

            await _unit.Save();
            return true;
        }
    }
}