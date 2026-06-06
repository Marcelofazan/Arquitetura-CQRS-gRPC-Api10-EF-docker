using Microsoft.EntityFrameworkCore;

namespace InfraEstrutura.Producao.DataModels.Data
{
    public class ProducaoAppDbContext : DbContext
    {
        public ProducaoAppDbContext(DbContextOptions<ProducaoAppDbContext> options)
       : base(options) { }

        public DbSet<ClienteDataModel> Clientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=ProducaoDB;Username=postgres;Password=Dgr8s99#",
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly("InfraEstrutura.Producao.DataModels"));
            }
        }
    }
}
