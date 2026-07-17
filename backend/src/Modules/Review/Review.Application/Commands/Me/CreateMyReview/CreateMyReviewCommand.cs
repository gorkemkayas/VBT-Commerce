using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Review.Domain.Enums;

namespace Review.Application.Commands.Me.CreateMyReview;

public record CreateMyReviewCommand(
    Guid SellableItemId,
    ReviewItemType SellableItemType,
    int Rating,
    string Comment) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Customer", "Admin"];
}
