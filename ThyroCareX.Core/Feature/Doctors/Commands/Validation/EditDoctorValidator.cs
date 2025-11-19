using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThyroCareX.Core.Feature.Doctors.Commands.Models;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Core.Feature.Doctors.Commands.Validation
{
    public class EditDoctorValidator: AbstractValidator<EditDoctorCommand>
    {
        private readonly IDoctorService _doctorService;
        public EditDoctorValidator(IDoctorService doctorService)
        {
            
            _doctorService = doctorService;
            ApplyValidationRules();
            ApplyCustomValidationRules();

        }
        public void ApplyValidationRules()
        {
            RuleFor (x=> x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");
          
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
        }
        public void ApplyCustomValidationRules()
        {
            RuleFor(x => x.Email)
                .MustAsync(async (model, Key, CancellationToken) => !await _doctorService.IsEmailExistExcludeSelf(Key, model.Id));
        }
    }
}
