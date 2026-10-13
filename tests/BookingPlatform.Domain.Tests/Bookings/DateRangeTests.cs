using BookingPlatform.Domain.Bookings;

namespace BookingPlatform.Domain.Tests.Bookings;

public class DateRangeTests
{
    private static DateOnly Day(int day) => new(2026, 11, day);

    [Fact]
    public void Nights_CountsDaysBetweenCheckInAndCheckOut()
    {
        var range = new DateRange(Day(10), Day(14));

        Assert.Equal(4, range.Nights);
    }

    [Theory]
    [InlineData(10, 10)]
    [InlineData(12, 10)]
    public void Constructor_RejectsCheckOutNotAfterCheckIn(int checkIn, int checkOut)
    {
        Assert.Throws<DomainException>(() => new DateRange(Day(checkIn), Day(checkOut)));
    }

    [Theory]
    [InlineData(12, 16, true)]
    [InlineData(8, 12, true)]
    [InlineData(11, 13, true)]
    [InlineData(5, 20, true)]
    [InlineData(10, 14, true)]
    [InlineData(14, 18, false)]
    [InlineData(6, 10, false)]
    [InlineData(20, 25, false)]
    public void Overlaps_DetectsIntersectionAndAllowsBackToBackStays(int checkIn, int checkOut, bool expected)
    {
        var existing = new DateRange(Day(10), Day(14));
        var candidate = new DateRange(Day(checkIn), Day(checkOut));

        Assert.Equal(expected, existing.Overlaps(candidate));
        Assert.Equal(expected, candidate.Overlaps(existing));
    }
}
