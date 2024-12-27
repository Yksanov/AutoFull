namespace AutoFull.Models;

public class Marca
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public string Description { get; set; }
    public string PathToPhoto { get; set; }
    
    public List<Auto> Autos { get; set; }
    public List<Cart> Carts { get; set; }
    public List<Feedback> Feedbacks { get; set; }
    public double AverageRating => Feedbacks.Any() ? Math.Round(Feedbacks.Average(x => x.Rating), 1) : 0.0;

    public Marca()
    {
        Autos = new List<Auto>();
        Carts = new List<Cart>();
        Feedbacks = new List<Feedback>();
    }
}