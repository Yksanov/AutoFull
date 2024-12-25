using System.ComponentModel.DataAnnotations;

namespace AutoFull.Models;

public class Auto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Название авто не можеть быть пустым")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Свет авто не можеть быть пустым")]
    public string Color { get; set; }
    
    [Required(ErrorMessage = "Год выпуска обязателен")]
    [Range(1990, 2100, ErrorMessage = "Укажите корректный год выпуска")]
    public int Year { get; set; }
    
    [Required(ErrorMessage = "Цена авто не можеть быть пустым")]
    public decimal Price { get; set; }
    [Required(ErrorMessage = "Описание авто не можеть быть пустым")]
    public string Description { get; set; }
    
    public int MarcaId { get; set; }
    public Marca? Marca { get; set; }
}