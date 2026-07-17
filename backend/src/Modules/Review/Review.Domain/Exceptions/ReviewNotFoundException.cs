using BuildingBlocks.Domain;

namespace Review.Domain.Exceptions;

public class ReviewNotFoundException : DomainException
{
    public ReviewNotFoundException(Guid id)
        : base($"Review '{id}' was not found.")
    {
    }
}
