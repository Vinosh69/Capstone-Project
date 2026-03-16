using System.Collections.Generic;

namespace RentAPlace.Models
{
    public class Feature
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public ICollection<PropertyFeature>? PropertyFeatures { get; set; }
    }
}