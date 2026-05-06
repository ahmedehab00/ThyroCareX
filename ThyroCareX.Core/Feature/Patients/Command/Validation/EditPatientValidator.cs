using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Feature.Patients.Command.Model;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Core.Feature.Patients.Command.Validation
{
    public class EditPatientValidator: AbstractValidator<EditPatientCommand>
    {
        private readonly IPatientService _patientService;
        public EditPatientValidator(IPatientService patientService)
        {
            _patientService = patientService;
            ApplyValidationRules();
        }

        public void ApplyValidationRules()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email is required.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Age)
                .GreaterThan(0).WithMessage("Age is required")
                .LessThanOrEqualTo(120).WithMessage("Age must be less than or equal to 120");
        }
    }
}
