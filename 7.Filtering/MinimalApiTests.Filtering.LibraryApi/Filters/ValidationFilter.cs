using FluentValidation;
using MinimalApiTests.Filtering.LibraryApi.Validators;

namespace MinimalApiTests.Filtering.LibraryApi.Filters;

public class ValidationFilter<T> : IRouteHandlerFilter where T : class
{
    private readonly IValidator<T> _validator;


    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context, RouteHandlerFilterDelegate next)
    {
        if (context.Parameters.SingleOrDefault(x => x?.GetType() == typeof(T)) is not T validatable)
        {
            return Results.BadRequest();
        }

        var validationResult = await _validator.ValidateAsync(validatable);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors.ToResponse());
        }
        
        return await next(context);
    }
}