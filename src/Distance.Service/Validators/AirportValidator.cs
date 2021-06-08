using Airports.Providers;
using FluentValidation;

namespace Distance.Service
{
    public class AirportValidator : AbstractValidator<Airport>
    {
        public AirportValidator()
        {
            RuleFor(a => a.Name).NotEmpty().MaximumLength(255);
            RuleFor(a => a.Latitude).NotEmpty().GreaterThanOrEqualTo(-90).LessThanOrEqualTo(90);
            RuleFor(a => a.Longitude).NotEmpty().GreaterThanOrEqualTo(-180).LessThanOrEqualTo(180);
            RuleFor(a => a.ElevationFt).NotEmpty().GreaterThanOrEqualTo(-13698).LessThanOrEqualTo(29029);
            RuleFor(a => a.IataCode).NotNull().SetValidator(new IataCodeValidator());
        }
    }
}
