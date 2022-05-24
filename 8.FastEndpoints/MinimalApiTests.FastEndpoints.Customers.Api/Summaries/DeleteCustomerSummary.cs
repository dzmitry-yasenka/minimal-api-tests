using FastEndpoints;
using MinimalApiTests.FastEndpoints.Customers.Api.Endpoints;

namespace MinimalApiTests.FastEndpoints.Customers.Api.Summaries;

public class DeleteCustomerSummary : Summary<DeleteCustomerEndpoint>
{
    public DeleteCustomerSummary()
    {
        Summary = "Deleted a customer the system";
        Description = "Deleted a customer the system";
        Response(204, "The customer was deleted successfully");
        Response(404, "The customer was not found in the system");
    }
}