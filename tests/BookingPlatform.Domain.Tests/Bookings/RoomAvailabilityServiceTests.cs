using BookingPlatform.Domain.Bookings;

namespace BookingPlatform.Domain.Tests.Bookings;

public class RoomAvailabilityServiceTests
{
    private static readonly DateTime Now = new(2026, 11, 1, 12, 0, 0);

    private readonly RoomAvailabilityService _service = new();
    private readonly Room _room = new(Guid.NewGuid(), Guid.NewGuid(), "101", 2, 1000m);

    private static DateRange Stay(int checkIn, int checkOut)
    {
        return new DateRange(new DateOnly(2026, 11, checkIn), new DateOnly(2026, 11, checkOut));
    }

    private Reservation Reserve(Room room, DateRange stay)
    {
        return Reservation.Create(room, Guid.NewGuid(), stay, 1, Now);
    }

    [Fact]
    public void IsAvailable_WhenNoReservations()
    {
        Assert.True(_service.IsAvailable(_room, Stay(10, 12), Array.Empty<Reservation>()));
    }

    [Fact]
    public void IsAvailable_FalseWhenActiveReservationOverlaps()
    {
        var existing = new[] { Reserve(_room, Stay(10, 14)) };

        Assert.False(_service.IsAvailable(_room, Stay(12, 16), existing));
    }

    [Fact]
    public void IsAvailable_TrueForBackToBackStay()
    {
        var existing = new[] { Reserve(_room, Stay(10, 14)) };

        Assert.True(_service.IsAvailable(_room, Stay(14, 16), existing));
    }

    [Fact]
    public void IsAvailable_IgnoresCancelledReservations()
    {
        var cancelled = Reserve(_room, Stay(10, 14));
        cancelled.Cancel();

        Assert.True(_service.IsAvailable(_room, Stay(11, 13), new[] { cancelled }));
    }

    [Fact]
    public void IsAvailable_IgnoresReservationsOfOtherRooms()
    {
        var otherRoom = new Room(Guid.NewGuid(), _room.HotelId, "102", 2, 1000m);
        var existing = new[] { Reserve(otherRoom, Stay(10, 14)) };

        Assert.True(_service.IsAvailable(_room, Stay(11, 13), existing));
    }
}
