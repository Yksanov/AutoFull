using Microsoft.AspNetCore.Identity;

namespace AutoFull.Models;

public class MyUser : IdentityUser<int>
{
    public string? PathAvatar { get; set; }
    public DateTime RegistrationDate { get; set; }
}