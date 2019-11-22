using FluentValidation;

namespace Api.Features.Search.GetAll
{
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.Phrase).NotNull().WithErrorCode("1");
            RuleFor(c => c.Phrase).NotEmpty().WithErrorCode("2");

            RuleFor(c => c.Limit).GreaterThan(0);
        }
    }
}