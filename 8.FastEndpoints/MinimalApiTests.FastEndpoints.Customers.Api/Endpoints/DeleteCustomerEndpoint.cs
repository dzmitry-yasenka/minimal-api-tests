using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using MinimalApiTests.FastEndpoints.Customers.Api.Contracts.Requests;
using MinimalApiTests.FastEndpoints.Customers.Api.Services;

namespace MinimalApiTests.FastEndpoints.Customers.Api.Endpoints;

[HttpDelete("customers/{id:guid}"), AllowAnonymous]
public class DeleteCustomerEndpoint : Endpoint<DeleteCustomerRequest>
{
    private readonly ICustomerService _customerService;

    public DeleteCustomerEndpoint(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public override async Task HandleAsync(DeleteCustomerRequest req, CancellationToken ct)
    {
        var deleted = await _customerService.DeleteAsync(req.Id);
        if (!deleted)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}