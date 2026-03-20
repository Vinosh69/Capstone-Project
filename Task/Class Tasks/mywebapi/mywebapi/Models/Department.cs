namespace mywebapi.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime  Date { get; set; } = DateTime.Now;
    }
}
