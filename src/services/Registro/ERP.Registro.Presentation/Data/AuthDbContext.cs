using ERP.Registro.Presentation.Abstractions;
using ERP.Registro.Presentation.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERP.Registro.Presentation.Data
{
    public class AuthDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>, IUnitOfWork
    {
        public DbSet<Empresa> Empresas { get; set; }
        public AuthDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Ignore<ValidationResult>();
            builder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        }

        public async Task<bool> CommitAsync()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
