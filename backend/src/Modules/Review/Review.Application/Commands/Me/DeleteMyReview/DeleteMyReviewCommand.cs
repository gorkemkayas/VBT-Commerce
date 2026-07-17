using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Review.Application.Commands.Me.DeleteMyReview;

public record DeleteMyReviewCommand(Guid ReviewId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
