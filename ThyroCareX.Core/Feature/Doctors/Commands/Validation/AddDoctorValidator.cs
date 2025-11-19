using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Feature.Doctors.Commands.Models;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Core.Feature.Doctors.Commands.Validation
{
    public class AddDoctorValidator : AbstractValidator<AddDoctorCommand>
    {
        private readonly IDoctorService _doctorService;
        public AddDoctorValidator(IDoctorService doctorService)
        {

            _doctorService = doctorService;
            ApplyValidationRules();

        }
        public void ApplyValidationRules()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
        }
        
    }
}
