namespace AutoFull.Models;

public class CartAuto
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public Cart Cart { get; set; }
    public int AutoId { get; set; }
    public Auto Auto { get; set; }
    public int Quantity { get; set; }
}