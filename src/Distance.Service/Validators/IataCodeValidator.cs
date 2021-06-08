using FluentValidation;

namespace Distance.Service
{
    public class IataCodeValidator : AbstractValidator<string>
    {
        public IataCodeValidator()
        {
            RuleFor(p => p).NotEmpty().Length(3).Matches("^[A-Z]+$");
        }
    }
}
