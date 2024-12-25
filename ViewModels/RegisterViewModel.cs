using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AutoFull.ViewModels;

public class RegisterViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "логин не может быть пустым")]
    [Remote(action: "CheckUserName", controller: "Validation", ErrorMessage = "Пользователь с таким именем пользователя уже существует")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Почта не может быть пустой")]
    [Remote(action: "CheckUserEmail", controller: "Validation", ErrorMessage = "Пользователь с таким почтой уже существует")]
    [EmailAddress(ErrorMessage = "Почта в некорретном формате!")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Номер телефона не можеть быть пустым")]
    [DataType(DataType.Password)]
    public string PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "Аватар не может быть пустьм")]
    public IFormFile Avatar { get; set; }
    
    [Required(ErrorMessage = "Пароль не может быть пустым")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Повтор пароля не может быть пустым")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}