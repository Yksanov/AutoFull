namespace AutoFull.Models;

public class Marca
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string Description { get; set; }
    public IFormFile PathToPhoto { get; set; }
    
    public List<Auto> Autos { get; set; }
    public List<Cart> Carts { get; set; }

    public Marca()
    {
        Autos = new List<Auto>();
        Carts = new List<Cart>();
    }
}