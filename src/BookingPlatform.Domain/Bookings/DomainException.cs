namespace BookingPlatform.Domain.Bookings;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
