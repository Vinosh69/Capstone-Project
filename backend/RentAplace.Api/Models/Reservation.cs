using System;

namespace RentAPlace.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int PropertyId { get; set; }

        public Property? Property { get; set; }

        public int RenterId { get; set; }

        public User? Renter { get; set; }

        public DateTime? CheckInDate { get; set; }

        public DateTime? CheckOutDate { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Rejected
    }
}