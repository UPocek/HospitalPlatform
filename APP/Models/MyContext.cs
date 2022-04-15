using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace APP.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
        }

        public DbSet<Other> Others { get; set; } = null!;
    }
}