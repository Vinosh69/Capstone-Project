using System.Collections.Generic;

namespace RentAPlace.Models
{
    public class Property
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public string Location { get; set; } = "";

        public string PropertyType { get; set; } = ""; // Flat, Villa, Apartment

        public decimal PricePerNight { get; set; }

        public double Rating { get; set; } = 0;

        public int OwnerId { get; set; }

        public User? Owner { get; set; }

        // Navigation Properties
        public ICollection<PropertyImage>? Images { get; set; }

        public ICollection<PropertyFeature>? PropertyFeatures { get; set; }

        public ICollection<Reservation>? Reservations { get; set; }

        public ICollection<Message>? Messages { get; set; }
    }
}