using FluentValidation;
using TaskManagement.API.Models.DTOs.UpdateRequest;
using TaskManagementAPI.Helpers;

namespace TaskManagement.API.Helpers.Validators.FluentValidators
{
    public class UpdateProjectValidator : AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Start_date)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Please set a valid Start Date");

            RuleFor(x => x.Status)
                .LessThan(Helper.MaxProjectStatus)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Please set a valid Project Status");

            RuleFor(x => x.End_date)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Please set valid End Date");
        }

    }
}
