using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Review.Application.Commands.Me.UpdateMyReview;

public record UpdateMyReviewCommand(Guid ReviewId, int Rating, string Comment) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
