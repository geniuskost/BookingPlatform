using BookingPlatform.Domain.Bookings;

namespace BookingPlatform.Domain.Tests.Bookings;

public class RoomTests
{
    [Fact]
    public void Constructor_TrimsNumber()
    {
        var room = new Room(Guid.NewGuid(), Guid.NewGuid(), " 101 ", 2, 1500m);

        Assert.Equal("101", room.Number);
    }

    [Theory]
    [InlineData("", 2, 1000)]
    [InlineData("  ", 2, 1000)]
    [InlineData("101", 0, 1000)]
    [InlineData("101", 2, 0)]
    [InlineData("101", 2, -50)]
    public void Constructor_RejectsInvalidValues(string number, int capacity, decimal price)
    {
        Assert.Throws<DomainException>(() => new Room(Guid.NewGuid(), Guid.NewGuid(), number, capacity, price));
    }
}
