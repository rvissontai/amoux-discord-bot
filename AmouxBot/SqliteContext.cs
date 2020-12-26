using Entidades;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class SqliteContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=amoux.db");
    }
}
