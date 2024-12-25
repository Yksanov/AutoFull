using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AutoFull.ViewModels;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "Имя пользователя не может быть пустым")]
    [Remote(action: "CheckUserName", controller: "Validation", ErrorMessage = "Пользователь с таким именем пользователя уже существует")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Почта пользователя не может быть пустой")]
    [Remote(action: "CheckUserEmail", controller: "Validation", ErrorMessage = "Пользователь с таким почтой уже существует")]
    [EmailAddress(ErrorMessage = "Почта в некорретном формате!")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Номер телефона не можеть быть пустым")]
    public string PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "Аватар не может быть пустьм")]
    public IFormFile Avatar { get; set; }
    
    [Required(ErrorMessage = "Пароль не может быть пустым")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Роль пользователя не можеть быть пустым")]
    public string Role { get; set; }
}