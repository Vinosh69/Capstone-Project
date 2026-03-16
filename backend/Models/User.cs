using System;
using System.Collections.Generic;

namespace RentAPlace.Models
{
    public class User
    {
        public int Id { get; set; }   // Primary Key

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        public string Role { get; set; } = ""; // Owner or Renter

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Property>? Properties { get; set; }

        public ICollection<Reservation>? Reservations { get; set; }

        public ICollection<Message>? SentMessages { get; set; }

        public ICollection<Message>? ReceivedMessages { get; set; }
    }
}