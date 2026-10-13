using BookingPlatform.Domain.Bookings;

namespace BookingPlatform.Domain.Tests.Bookings;

public class ReservationTests
{
    private static readonly DateTime Now = new(2026, 11, 1, 12, 0, 0);

    private static Room CreateRoom(int capacity = 2, decimal price = 1200m)
    {
        return new Room(Guid.NewGuid(), Guid.NewGuid(), "101", capacity, price);
    }

    private static DateRange Stay(int checkIn, int checkOut)
    {
        return new DateRange(new DateOnly(2026, 11, checkIn), new DateOnly(2026, 11, checkOut));
    }

    private static Reservation CreateReservation(Room? room = null, DateRange? stay = null, int guests = 2)
    {
        return Reservation.Create(room ?? CreateRoom(), Guid.NewGuid(), stay ?? Stay(10, 13), guests, Now);
    }

    [Fact]
    public void Create_CalculatesTotalPriceFromNightsAndRoomPrice()
    {
        var reservation = CreateReservation(CreateRoom(price: 1200m), Stay(10, 13));

        Assert.Equal(3600m, reservation.TotalPrice);
        Assert.Equal(1200m, reservation.PricePerNight);
    }

    [Fact]
    public void Create_StartsPendingAndActive()
    {
        var reservation = CreateReservation();

        Assert.Equal(ReservationStatus.Pending, reservation.Status);
        Assert.True(reservation.IsActive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(3)]
    public void Create_RejectsGuestCountOutsideRoomCapacity(int guests)
    {
        var room = CreateRoom(capacity: 2);

        Assert.Throws<DomainException>(() => CreateReservation(room, guests: guests));
    }

    [Fact]
    public void Confirm_MovesPendingToConfirmed()
    {
        var reservation = CreateReservation();

        reservation.Confirm();

        Assert.Equal(ReservationStatus.Confirmed, reservation.Status);
    }

    [Fact]
    public void Confirm_RejectsAlreadyConfirmedReservation()
    {
        var reservation = CreateReservation();
        reservation.Confirm();

        Assert.Throws<DomainException>(reservation.Confirm);
    }

    [Fact]
    public void Complete_RequiresConfirmedReservation()
    {
        var reservation = CreateReservation();

        Assert.Throws<DomainException>(reservation.Complete);

        reservation.Confirm();
        reservation.Complete();

        Assert.Equal(ReservationStatus.Completed, reservation.Status);
    }

    [Fact]
    public void Cancel_WorksForPendingAndConfirmed()
    {
        var pending = CreateReservation();
        var confirmed = CreateReservation();
        confirmed.Confirm();

        pending.Cancel();
        confirmed.Cancel();

        Assert.Equal(ReservationStatus.Cancelled, pending.Status);
        Assert.Equal(ReservationStatus.Cancelled, confirmed.Status);
        Assert.False(pending.IsActive);
    }

    [Fact]
    public void Cancel_RejectsCancelledAndCompletedReservations()
    {
        var cancelled = CreateReservation();
        cancelled.Cancel();
        var completed = CreateReservation();
        completed.Confirm();
        completed.Complete();

        Assert.Throws<DomainException>(cancelled.Cancel);
        Assert.Throws<DomainException>(completed.Cancel);
    }

    [Theory]
    [InlineData(240, true)]
    [InlineData(48, true)]
    [InlineData(47.9, false)]
    [InlineData(12, false)]
    public void IsFreeCancellation_RequiresAtLeast48HoursBeforeCheckIn(double hoursBeforeCheckIn, bool expected)
    {
        var reservation = CreateReservation(stay: Stay(10, 12));
        var checkIn = new DateTime(2026, 11, 10);
        var now = checkIn - TimeSpan.FromHours(hoursBeforeCheckIn);

        Assert.Equal(expected, reservation.IsFreeCancellation(now));
    }
}
