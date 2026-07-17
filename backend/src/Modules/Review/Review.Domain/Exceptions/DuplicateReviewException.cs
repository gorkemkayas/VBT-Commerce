using BuildingBlocks.Domain;

namespace Review.Domain.Exceptions;

public class DuplicateReviewException : DomainException
{
    public DuplicateReviewException(Guid userId, Guid sellableItemId)
        : base($"User '{userId}' has already reviewed item '{sellableItemId}'.")
    {
    }
}
