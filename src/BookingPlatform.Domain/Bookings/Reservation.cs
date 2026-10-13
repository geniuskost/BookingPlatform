namespace BookingPlatform.Domain.Bookings;

public class Reservation
{
    public static readonly TimeSpan FreeCancellationPeriod = TimeSpan.FromHours(48);

    private Reservation(Guid id, Room room, Guid guestId, DateRange stay, int guests, DateTime createdAt)
    {
        Id = id;
        RoomId = room.Id;
        GuestId = guestId;
        Stay = stay;
        Guests = guests;
        PricePerNight = room.PricePerNight;
        TotalPrice = room.PricePerNight * stay.Nights;
        Status = ReservationStatus.Pending;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public Guid RoomId { get; }

    public Guid GuestId { get; }

    public DateRange Stay { get; }

    public int Guests { get; }

    public decimal PricePerNight { get; }

    public decimal TotalPrice { get; }

    public ReservationStatus Status { get; private set; }

    public DateTime CreatedAt { get; }

    public bool IsActive => Status is ReservationStatus.Pending or ReservationStatus.Confirmed;

    public static Reservation Create(Room room, Guid guestId, DateRange stay, int guests, DateTime createdAt)
    {
        if (guests < 1)
        {
            throw new DomainException("Потрібен принаймні один гість");
        }

        if (guests > room.Capacity)
        {
            throw new DomainException($"Номер вміщує щонайбільше {room.Capacity} гостей");
        }

        return new Reservation(Guid.NewGuid(), room, guestId, stay, guests, createdAt);
    }

    public void Confirm()
    {
        if (Status != ReservationStatus.Pending)
        {
            throw new DomainException("Підтвердити можна лише бронювання в очікуванні");
        }

        Status = ReservationStatus.Confirmed;
    }

    public void Complete()
    {
        if (Status != ReservationStatus.Confirmed)
        {
            throw new DomainException("Завершити можна лише підтверджене бронювання");
        }

        Status = ReservationStatus.Completed;
    }

    public void Cancel()
    {
        if (!IsActive)
        {
            throw new DomainException("Скасувати можна лише активне бронювання");
        }

        Status = ReservationStatus.Cancelled;
    }

    public bool IsFreeCancellation(DateTime now)
    {
        var checkIn = Stay.CheckIn.ToDateTime(TimeOnly.MinValue);

        return checkIn - now >= FreeCancellationPeriod;
    }
}
