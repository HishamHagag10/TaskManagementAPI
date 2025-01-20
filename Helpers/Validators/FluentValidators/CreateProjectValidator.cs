using FluentValidation;
using TaskManagement.API.Models.DTOs.AddRequest;
using TaskManagementAPI.Helpers;

namespace TaskManagement.API.Helpers.Validators.FluentValidators
{
    public class CreateProjectValidator : AbstractValidator<AddProjectDto>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Start_date)
                .NotEmpty()
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Please set a valid Start Date");

            RuleFor(x => x.Status)
                .NotEmpty()
                .LessThanOrEqualTo(Helper.MaxProjectStatus)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Please set a valid Project Status");

            RuleFor(x => x.End_date)
                .NotEmpty()
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Please set valid End Date Date");

            RuleFor(x => x.End_date)
                .GreaterThan(x => x.Start_date.AddDays(7))
                .WithMessage("Please set valid End Date, It must be greater than the start Date by at least a Week");
        }

    }
}
