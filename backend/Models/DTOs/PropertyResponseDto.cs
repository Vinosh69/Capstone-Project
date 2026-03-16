namespace RentAPlace.Models.DTOs
{
    public class PropertyResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Location { get; set; } = "";
        public string PropertyType { get; set; } = "";
        public decimal PricePerNight { get; set; }
        public double Rating { get; set; }
        public int OwnerId { get; set; }
        public OwnerSummaryDto? Owner { get; set; }
        public List<PropertyImageDto> Images { get; set; } = new();
    }
}
