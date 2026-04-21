using FluentValidation;
using Lab2.DTOs.StudentDTOS;

namespace Lab3.Validator
{
    public class CreatedStudentValidator : AbstractValidator<CreatedStudentDTO>
    {
        public CreatedStudentValidator()
        {
            RuleFor(x => x.StFname).NotEmpty().WithMessage("First Name is required").MaximumLength(50).Matches(@"^[a-zA-Z\s\-]+$").WithMessage("First Name can only contain letters, spaces, and hyphens."); ;
            RuleFor(x => x.StLname).NotEmpty().WithMessage("Last Name is required").MaximumLength(50).Matches(@"^[a-zA-Z\s\-]+$").WithMessage("Last Name can only contain letters, spaces, and hyphens.");
            RuleFor(x => x.age).InclusiveBetween(18, 100).WithMessage("Age must be between 18 and 100");    
            RuleFor(x => x.deptID).GreaterThan(0).WithMessage("Department ID must be valid");
        }
    }
}
