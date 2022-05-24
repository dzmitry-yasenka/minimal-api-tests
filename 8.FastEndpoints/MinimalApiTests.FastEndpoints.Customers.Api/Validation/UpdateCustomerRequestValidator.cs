using FluentValidation;
using MinimalApiTests.FastEndpoints.Customers.Api.Contracts.Requests;

namespace MinimalApiTests.FastEndpoints.Customers.Api.Validation;

public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.DateOfBirth).NotEmpty();
    }
}