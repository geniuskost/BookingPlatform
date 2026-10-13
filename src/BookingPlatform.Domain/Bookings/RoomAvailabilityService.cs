namespace BookingPlatform.Domain.Bookings;

public class RoomAvailabilityService
{
    public bool IsAvailable(Room room, DateRange stay, IEnumerable<Reservation> reservations)
    {
        return !reservations.Any(reservation =>
            reservation.RoomId == room.Id
            && reservation.IsActive
            && reservation.Stay.Overlaps(stay));
    }
}
