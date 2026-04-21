using AutoMapper;
using Lab2.DTOs.DepartmentDTOS;
using Lab2.DTOs.StudentDTOS;
using Lab3.DTOs.DepartmentDTOS;
using Lab3.Models;

namespace Lab2.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // Student → DTO
            CreateMap<Student, StudentDTO>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.StAge))
                .AfterMap((src, dest) =>
                {
                    dest.FullName = $"{src.StFname} {src.StLname}";
                    dest.DepartmentName = src.Dept != null ? src.Dept.DeptName : null;
                    dest.SupervisorName = src.StSuperNavigation != null
                        ? $"{src.StSuperNavigation.StFname} {src.StSuperNavigation.StLname}"
                        : null;
                });

            // Department → DepartmentDTO
            CreateMap<Department, DepartmentDTO>()
                .ForMember(dest => dest.deptName, opt => opt.MapFrom(src => src.DeptName))
                .ForMember(dest => dest.deptDescription, opt => opt.MapFrom(src => src.DeptDesc))
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Students.Count));

            // 🔥 THIS IS THE MISSING ONE
            CreateMap<Department, ReadDepartmentDTO>();

            // Create DTO → Entity
            CreateMap<CreatedStudentDTO, Student>()
                .ForMember(dest => dest.StId, opt => opt.Ignore())
                .ForMember(dest => dest.StFname, opt => opt.MapFrom(src => src.StFname))
                .ForMember(dest => dest.StLname, opt => opt.MapFrom(src => src.StLname))
                .ForMember(dest => dest.StAge, opt => opt.MapFrom(src => src.age))
                .ForMember(dest => dest.DeptId, opt => opt.MapFrom(src => src.deptID));

            CreateMap<CreatedDepartmentDTO, Department>();
        }
    }
}