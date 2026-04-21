using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Lab3.Models;

public partial class Department
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DeptId { get; set; }

    public string? DeptName { get; set; }

    public string? DeptDesc { get; set; }

    public string? DeptLocation { get; set; }

    public int? DeptManager { get; set; }

    public DateOnly? ManagerHiredate { get; set; }

    public virtual Instructor? DeptManagerNavigation { get; set; }

    public virtual ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
    [JsonIgnore]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
