namespace AutoFull.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public MyUser? User { get; set; }
    public int MarcaId { get; set; }
    public Marca? Marca { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; }
    public double TotalPrice { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    
    public List<Auto> Autos { get; set; } = new List<Auto>();
}