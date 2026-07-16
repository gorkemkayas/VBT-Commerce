using Customer.Application.Commands.GuestCustomers.CreateGuestCustomer;
using Customer.Application.Common;
using Customer.Application.Queries.GuestCustomers.GetGuestCustomerById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.Customers;

[ApiController]
[Route("api/guest-customers")]
public class GuestCustomersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateGuestCustomerRequest request, CancellationToken cancellationToken)
    {
        var guestCustomerId = await sender.Send(
            new CreateGuestCustomerCommand(request.FirstName, request.LastName, request.Email, request.PhoneNumber),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { guestCustomerId }, guestCustomerId);
    }

    [HttpGet("{guestCustomerId:guid}")]
    public async Task<ActionResult<GuestCustomerDto>> GetById(Guid guestCustomerId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetGuestCustomerByIdQuery(guestCustomerId), cancellationToken);
        return Ok(result);
    }
}
