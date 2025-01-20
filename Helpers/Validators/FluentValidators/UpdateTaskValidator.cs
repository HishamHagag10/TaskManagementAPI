using FluentValidation;
using TaskManagement.API.Models.DTOs.UpdateRequest;
using TaskManagementAPI.Helpers;

namespace TaskManagement.API.Helpers.Validators.FluentValidators
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.Title)
               .MaximumLength(50);
            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.Priority)
                .LessThanOrEqualTo(Helper.MaxPriority)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Please set a valid Priority");

            RuleFor(x => x.Status)
                .LessThanOrEqualTo(Helper.MaxTaskStatus)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Please set a valid Task Status");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Please set vlaid Due Date");

        }
    }
}
