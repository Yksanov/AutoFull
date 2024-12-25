namespace AutoFull.Models;

public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public MyUser? User { get; set; }
    public int MarcaId { get; set; }
    public Marca? Marca { get; set; }
    public List<CartAuto> CartAutos { get; set; }

    public Cart()
    {
        CartAutos = new List<CartAuto>();
    }
}