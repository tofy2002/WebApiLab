using Lab2.DTOs.DepartmentDTOS;
using Lab2.Repository;
using Lab3.DTOs.DepartmentDTOS;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentService _service;

        public DepartmentController(DepartmentService service)
        {
            _service = service;
        }

        // ================= GET ALL =================
        [HttpGet]
        [EndpointSummary("Get all departments")]
        [EndpointDescription("Retrieve all departments from database. ex: /api/department")]
        [ProducesResponseType(typeof(List<ReadDepartmentDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var data = await _service.GetAll();
            return Ok(data);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        [EndpointSummary("Get department by ID")]
        [EndpointDescription("Retrieve a single department by its ID. ex: /api/department/1")]
        [ProducesResponseType(typeof(ReadDepartmentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var dept = await _service.GetById(id);

            if (dept == null)
                return NotFound();

            return Ok(dept);
        }

        // ================= CREATE =================
        [HttpPost]
        [EndpointSummary("Create department")]
        [EndpointDescription("Create a new department in the database")]
        [ProducesResponseType(typeof(ReadDepartmentDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(CreatedDepartmentDTO dto)
        {
            var department = await _service.Add(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = department.DeptId },
                department
            );
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        [EndpointSummary("Update department")]
        [EndpointDescription("Update an existing department by ID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, CreatedDepartmentDTO dto)
        {
            var updated = await _service.Update(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        [EndpointSummary("Delete department")]
        [EndpointDescription("Delete a department by ID")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.Delete(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}