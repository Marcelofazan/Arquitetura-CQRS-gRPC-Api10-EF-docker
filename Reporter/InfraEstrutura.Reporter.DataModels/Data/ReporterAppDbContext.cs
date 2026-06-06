using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraEstrutura.Reporter.DataModels.Data
{
    public class ReporterAppDbContext : DbContext
    {
        public ReporterAppDbContext(DbContextOptions<ReporterAppDbContext> options)
        : base(options) { }

        public DbSet<ClienteDataModel> Clientes { get; set; }
    }
}
