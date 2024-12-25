using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoFull.Models;

public class AutoFullDbContext : IdentityDbContext<MyUser, IdentityRole<int>, int>
{
    public DbSet<MyUser> Users { get; set; }
    public DbSet<Marca> Marcas { get; set; }
    public DbSet<Auto> Autos { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartAuto> CartAutos { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    public AutoFullDbContext(DbContextOptions<AutoFullDbContext> options) : base(options) {}
}