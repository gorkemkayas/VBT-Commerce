using BuildingBlocks.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Abstractions;
using Review.Domain.Exceptions;

namespace Review.Application.Commands.Me.UpdateMyReview;

public class UpdateMyReviewCommandHandler(IReviewDbContext dbContext, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateMyReviewCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMyReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var review = await dbContext.Reviews.FirstOrDefaultAsync(
            r => r.Id == request.ReviewId && r.UserId == userId, cancellationToken)
            ?? throw new ReviewNotFoundException(request.ReviewId);

        review.Update(request.Rating, request.Comment);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
