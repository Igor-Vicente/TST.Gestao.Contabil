using ERP.Registro.Presentation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Registro.Presentation.Data.Mappings
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.EmpresaId)
                .IsRequired();

            builder.Property(u => u.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.CriadoEm)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.Cargo)
              .IsRequired()
              .HasConversion<string>()   // armazena o nome do enum, ex: "Colaborador"
              .HasMaxLength(20);
        }
    }
}
