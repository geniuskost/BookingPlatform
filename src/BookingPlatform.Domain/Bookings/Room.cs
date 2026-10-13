namespace BookingPlatform.Domain.Bookings;

public class Room
{
    public Room(Guid id, Guid hotelId, string number, int capacity, decimal pricePerNight)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException("Номер кімнати обов'язковий");
        }

        if (capacity < 1)
        {
            throw new DomainException("Місткість номера має бути не менше одного гостя");
        }

        if (pricePerNight <= 0)
        {
            throw new DomainException("Ціна за ніч має бути додатною");
        }

        Id = id;
        HotelId = hotelId;
        Number = number.Trim();
        Capacity = capacity;
        PricePerNight = pricePerNight;
    }

    public Guid Id { get; }

    public Guid HotelId { get; }

    public string Number { get; }

    public int Capacity { get; }

    public decimal PricePerNight { get; }
}
