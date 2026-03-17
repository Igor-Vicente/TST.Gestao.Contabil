using ERP.Registro.Presentation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Registro.Presentation.Data.Mappings
{
    public class EmpresaMapping : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.ToTable("Empresas");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Cnpj)
                .IsRequired()
                .HasMaxLength(14)
                .IsUnicode(false);

            builder.HasIndex(e => e.Cnpj)
                .IsUnique();

            builder.Property(e => e.RazaoSocial)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.NomeFantasia)
                .HasMaxLength(150);

            builder.Property(e => e.RegimeTributario)
                .IsRequired()
                .HasConversion<string>()   // armazena o nome do enum, ex: "SimplesNacional"
                .HasMaxLength(20);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(e => e.CriadoEm)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relacionamento com Usuarios
            builder.HasMany(e => e.Usuarios)
                .WithOne(u => u.Empresa)
                .HasForeignKey(u => u.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
