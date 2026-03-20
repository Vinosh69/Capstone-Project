using System.ComponentModel.DataAnnotations;

namespace REST_API_Assignment.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Range(1888, 2100)]
        public int ReleaseYear { get; set; }

        [Required]
        public int DirectorId { get; set; }

        public Director? Director { get; set; }
    }
}