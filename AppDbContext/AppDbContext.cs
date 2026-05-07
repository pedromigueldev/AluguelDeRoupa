using AluguelRoupa.Models;
using Microsoft.EntityFrameworkCore;

namespace AluguelRoupa.AppDbContext;
public class Context(DbContextOptions<Context> options) : DbContext(options)
{
    public required DbSet<Clothes> Clothes { get; set; }
}