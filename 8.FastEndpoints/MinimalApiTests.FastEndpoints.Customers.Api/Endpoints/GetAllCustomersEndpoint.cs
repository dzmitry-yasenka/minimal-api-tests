using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using MinimalApiTests.FastEndpoints.Customers.Api.Contracts.Responses;
using MinimalApiTests.FastEndpoints.Customers.Api.Mapping;
using MinimalApiTests.FastEndpoints.Customers.Api.Services;

namespace MinimalApiTests.FastEndpoints.Customers.Api.Endpoints;

[HttpGet("customers"), AllowAnonymous]
public class GetAllCustomersEndpoint : EndpointWithoutRequest<GetAllCustomersResponse>
{
    private readonly ICustomerService _customerService;

    public GetAllCustomersEndpoint(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var customers = await _customerService.GetAllAsync();
        var customersResponse = customers.ToCustomersResponse();
        await SendOkAsync(customersResponse, ct);
    }
}