using FluentValidation;
using TaskManagement.API.Models.DTOs.AddRequest;
using TaskManagementAPI.Helpers;

namespace TaskManagement.API.Helpers.Validators.FluentValidators
{
    public class CreateTaskValidator : AbstractValidator<AddTaskDto>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(50);
            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(1000);

            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.TagsIds).NotEmpty();

            RuleFor(x => x.Priority)
                .NotEmpty()
                .LessThanOrEqualTo(Helper.MaxPriority)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Please set a valid Priority");

            RuleFor(x => x.Status)
                .NotEmpty()
                .LessThanOrEqualTo(Helper.MaxTaskStatus)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Please set a valid Task Status");

            RuleFor(x => x.DueDate)
                .NotEmpty()
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Please set valid Due Date");

        }

    }
}
