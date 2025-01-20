using FluentValidation;
using TaskManagement.API.Models.DTOs.AddRequest;

namespace TaskManagement.API.Helpers.Validators.FluentValidators
{
    public class CreateNotificationsValidator: 
        AbstractValidator<AddNotificationDto>
    {
        public CreateNotificationsValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("Message is required")
                .MaximumLength(200)
                .WithMessage("Message must not exceed 200 characters");

            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Type is required")
                .MaximumLength(50)
                .WithMessage("Type must not exceed 50 characters");
            
            RuleFor(x => x.ToUserId)
                .NotEmpty()
                .WithMessage("UserId is required");
        }
    }
}
