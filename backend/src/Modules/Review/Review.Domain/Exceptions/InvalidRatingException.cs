using BuildingBlocks.Domain;

namespace Review.Domain.Exceptions;

public class InvalidRatingException : DomainException
{
    public InvalidRatingException(int rating)
        : base($"Rating '{rating}' is invalid. Rating must be between 1 and 5.")
    {
    }
}
