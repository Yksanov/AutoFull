using AutoFull.Models;

namespace AutoFull.ViewModels;

public class IndexViewModel
{
    public MyUser CurrentUser { get; set; }
    public List<Auto> Autos { get; set; }
    public List<Marca> Marcas { get; set; }
    
    public PageViewModel PageViewModelForAuto { get; set; }
    public PageViewModel PageViewModelForMarca { get; set; }
}