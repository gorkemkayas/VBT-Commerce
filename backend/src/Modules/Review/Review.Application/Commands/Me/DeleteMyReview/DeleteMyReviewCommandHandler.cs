using BuildingBlocks.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Abstractions;
using Review.Domain.Exceptions;

namespace Review.Application.Commands.Me.DeleteMyReview;

public class DeleteMyReviewCommandHandler(IReviewDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<DeleteMyReviewCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMyReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var review = await dbContext.Reviews.FirstOrDefaultAsync(
            r => r.Id == request.ReviewId && r.UserId == userId, cancellationToken)
            ?? throw new ReviewNotFoundException(request.ReviewId);

        dbContext.Reviews.Remove(review);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
