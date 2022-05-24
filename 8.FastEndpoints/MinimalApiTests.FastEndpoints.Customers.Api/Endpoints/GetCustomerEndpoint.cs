﻿using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using MinimalApiTests.FastEndpoints.Customers.Api.Contracts.Requests;
using MinimalApiTests.FastEndpoints.Customers.Api.Contracts.Responses;
using MinimalApiTests.FastEndpoints.Customers.Api.Mapping;
using MinimalApiTests.FastEndpoints.Customers.Api.Services;

namespace MinimalApiTests.FastEndpoints.Customers.Api.Endpoints;

[HttpGet("customers/{id:guid}"), AllowAnonymous]
public class GetCustomerEndpoint : Endpoint<GetCustomerRequest, CustomerResponse>
{
    private readonly ICustomerService _customerService;

    public GetCustomerEndpoint(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public override async Task HandleAsync(GetCustomerRequest req, CancellationToken ct)
    {
        var customer = await _customerService.GetAsync(req.Id);

        if (customer is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var customerResponse = customer.ToCustomerResponse();
        await SendOkAsync(customerResponse, ct);
    }
}