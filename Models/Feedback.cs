namespace AutoFull.Models;

public class Feedback
{
    public int Id { get; set; }
    public string Text { get; set; }
    public  int Rating { get; set; }
    public  DateTime CreationDate { get; set; }
    public int MarcaId { get; set; }
    public Marca? Marca { get; set; }
    public int UserId { get; set; }
    public MyUser? User { get; set; }
}