using System.ComponentModel.DataAnnotations.Schema;
using WiproWebApp.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    [ForeignKey("categ")]
    public int Categid { get; set; }
    public Category categ { get; set; }
}