using MinimalApiTests.FastEndpoints.Customers.Api.Contracts.Data;
using MinimalApiTests.FastEndpoints.Customers.Api.Domain;
using MinimalApiTests.FastEndpoints.Customers.Api.Domain.Common;

namespace MinimalApiTests.FastEndpoints.Customers.Api.Mapping;

public static class DtoToDomainMapper
{
    public static Customer ToCustomer(this CustomerDto customerDto)
    {
        return new Customer
        {
            Id = CustomerId.From(Guid.Parse(customerDto.Id)),
            Email = EmailAddress.From(customerDto.Email),
            Username = Username.From(customerDto.Username),
            FullName = FullName.From(customerDto.FullName),
            DateOfBirth = DateOfBirth.From(DateOnly.FromDateTime(customerDto.DateOfBirth))
        };
    }
}