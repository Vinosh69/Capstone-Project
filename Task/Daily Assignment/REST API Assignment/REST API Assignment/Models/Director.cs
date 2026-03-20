using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace REST_API_Assignment.Models
{
    public class Director
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}