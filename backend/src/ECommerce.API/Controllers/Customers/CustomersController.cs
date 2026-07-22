using Customer.Application.Commands.Addresses.AddMyCustomerAddress;
using Customer.Application.Commands.Addresses.RemoveMyCustomerAddress;
using Customer.Application.Commands.Addresses.SetDefaultMyCustomerAddress;
using Customer.Application.Commands.Addresses.UpdateMyCustomerAddress;
using Customer.Application.Commands.Profile.CreateMyCustomerProfile;
using Customer.Application.Commands.Profile.UpdateMyCustomerProfile;
using Customer.Application.Common;
using Customer.Application.Queries.Admin.GetCustomerByUserId;
using Customer.Application.Queries.Admin.GetCustomersList;
using Customer.Application.Queries.Profile.GetMyCustomerProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using ECommerce.API.Controllers.Customers.Requests;

namespace ECommerce.API.Controllers.Customers;

[ApiController]
[Route("api/customers")]
public class CustomersController(ISender sender) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<CustomerDto>> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMyCustomerProfileQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("me")]
    public async Task<ActionResult<Guid>> CreateMyProfile(CreateCustomerProfileRequest request, CancellationToken cancellationToken)
    {
        var customerId = await sender.Send(
            new CreateMyCustomerProfileCommand(request.PhoneNumber, request.DateOfBirth), cancellationToken);

        return CreatedAtAction(nameof(GetMyProfile), null, customerId);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(UpdateCustomerProfileRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateMyCustomerProfileCommand(request.PhoneNumber, request.DateOfBirth), cancellationToken);
        return NoContent();
    }

    [HttpPost("me/addresses")]
    public async Task<ActionResult<Guid>> AddMyAddress(AddCustomerAddressRequest request, CancellationToken cancellationToken)
    {
        var addressId = await sender.Send(
            new AddMyCustomerAddressCommand(
                request.Label, request.RecipientName, request.PhoneNumber, request.Country, request.City,
                request.District, request.PostalCode, request.AddressLine1, request.AddressLine2, request.IsDefault),
            cancellationToken);

        return CreatedAtAction(nameof(GetMyProfile), null, addressId);
    }

    [HttpPut("me/addresses/{addressId:guid}")]
    public async Task<IActionResult> UpdateMyAddress(Guid addressId, UpdateCustomerAddressRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateMyCustomerAddressCommand(
                addressId, request.Label, request.RecipientName, request.PhoneNumber, request.Country, request.City,
                request.District, request.PostalCode, request.AddressLine1, request.AddressLine2, request.IsDefault),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("me/addresses/{addressId:guid}")]
    public async Task<IActionResult> RemoveMyAddress(Guid addressId, CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveMyCustomerAddressCommand(addressId), cancellationToken);
        return NoContent();
    }

    [HttpPost("me/addresses/{addressId:guid}/set-default")]
    public async Task<IActionResult> SetDefaultMyAddress(Guid addressId, CancellationToken cancellationToken)
    {
        await sender.Send(new SetDefaultMyCustomerAddressCommand(addressId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<CustomerDto>> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCustomerByUserIdQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CustomerListItemDto>>> GetList(
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCustomersListQuery(pageNumber == 0 ? 1 : pageNumber, pageSize == 0 ? 20 : pageSize), cancellationToken);

        return Ok(result);
    }
}
