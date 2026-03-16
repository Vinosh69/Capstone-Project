namespace RentAPlace.Models.DTOs
{
    public class ReservationResponseDto
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int RenterId { get; set; }
        public string Status { get; set; } = "Pending";

        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }

        public ReservationPropertyDto? Property { get; set; }
        public ReservationRenterDto? Renter { get; set; }
    }

    public class ReservationPropertyDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Location { get; set; } = "";
        public string PropertyType { get; set; } = "";
        public decimal PricePerNight { get; set; }
        public int OwnerId { get; set; }
    }

    public class ReservationRenterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }
}

