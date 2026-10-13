namespace BookingPlatform.Domain.Bookings;

public sealed record DateRange
{
    public DateRange(DateOnly checkIn, DateOnly checkOut)
    {
        if (checkOut <= checkIn)
        {
            throw new DomainException("Дата виїзду має бути пізніше за дату заїзду");
        }

        CheckIn = checkIn;
        CheckOut = checkOut;
    }

    public DateOnly CheckIn { get; }

    public DateOnly CheckOut { get; }

    public int Nights => CheckOut.DayNumber - CheckIn.DayNumber;

    public bool Overlaps(DateRange other)
    {
        return CheckIn < other.CheckOut && other.CheckIn < CheckOut;
    }
}
