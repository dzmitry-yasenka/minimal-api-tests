using FastEndpoints;
using MinimalApiTests.FastEndpoints.Customers.Api.Contracts.Responses;
using MinimalApiTests.FastEndpoints.Customers.Api.Endpoints;

namespace MinimalApiTests.FastEndpoints.Customers.Api.Summaries;

public class GetAllCustomersSummary : Summary<GetAllCustomersEndpoint>
{
    public GetAllCustomersSummary()
    {
        Summary = "Returns all the customers in the system";
        Description = "Returns all the customers in the system";
        Response<GetAllCustomersResponse>(200, "All customers in the system are returned");
    }
}