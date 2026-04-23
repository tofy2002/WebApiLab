using AutoMapper;
using Lab2.DTOs.StudentDTOS;
using Lab2.Repository;
using Lab3.DTOs;
using Lab3.Filters;
using Lab3.Models;
using Lab3.Services;
using Lab3.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lab3.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogActionFilter))]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _Service;  
        private readonly ILogger<StudentController> Logger;

        public StudentController(IStudentService StudentService,ILogger<StudentController> logger)
        {
            _Service = StudentService;
            Logger = logger;
        }
        [EndpointSummary("select all students")]
        [ServiceFilter(typeof(PersonActionFilter))]
        [HttpGet]
        public async Task<IActionResult> Get(string? search, int page = 1, int pageSize = 5,string? sortby = "fname", string? sortDir = "asc")
        {
            var result=await _Service.GetAll(search, page, pageSize, sortby, sortDir);
            return Ok(new ApiResponse<PagedResponse<StudentDTO>>
            {
                Success = true,
                Message = "Students retrieved successfully",
                Data = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> Add(CreatedStudentDTO stdo)
        {
            Logger.LogInformation("Adding a new student with name: {FirstName} {LastName}", stdo.StFname, stdo.StLname);
            var result= await _Service.Add(stdo);
            Logger.LogInformation("Successfully added student with ID: {Id}", result.StId);   
            return CreatedAtAction(nameof(GetById), new { id=result.StId}, new ApiResponse<StudentDTO>
            {
                Success = true,
                Message = "Student added successfully",
                Data = result
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreatedStudentDTO stdo)
        {
            await _Service.Update(id, stdo);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Student updated successfully",
                Data = true
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _Service.Delete(id);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Student deleted successfully",
                Data = true
            });
        } 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result= await _Service.GetById(id);
            return Ok(new ApiResponse<StudentDTO>
            {
                Success = true,
                Message = "Student retrieved successfully",
                Data = result
            });
        }
    }
}
