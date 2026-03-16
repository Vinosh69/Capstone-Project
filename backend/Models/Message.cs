using System;

namespace RentAPlace.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public int PropertyId { get; set; }

        public string MessageText { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? Sender { get; set; }

        public User? Receiver { get; set; }

        public Property? Property { get; set; }
    }
}