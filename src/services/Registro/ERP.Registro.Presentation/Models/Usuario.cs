using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace ERP.Registro.Presentation.Models
{
    public enum Cargo
    {
        AdministradorGeral = 1,
        Gestor = 2,
        Colaborador = 3,
    }

    public class Usuario : IdentityUser<int>
    {
        public string Nome { get; private set; }
        public int EmpresaId { get; private set; }
        public Cargo Cargo { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public ValidationResult ValidationResult { get; private set; }


        // Navegação
        public Empresa Empresa { get; set; }

        public Usuario(string nome, string email, int empresaId, Cargo cargo)
        {
            Nome = nome;
            Email = email;
            EmpresaId = empresaId;
            Cargo = cargo;
            Ativo = true;
            CriadoEm = DateTime.UtcNow;
            UserName = email; // Definindo UserName como email para compatibilidade com Identity
        }

        public bool IsValid()
        {
            ValidationResult = new UsuarioValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            // ── Nome ──────────────────────────────────────────────────────
            RuleFor(u => u.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres.")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres.");

            // ── Email ─────────────────────────────────────────────────────
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório.")
                .MaximumLength(150).WithMessage("E-mail deve ter no máximo 150 caracteres.")
                .EmailAddress().WithMessage("E-mail inválido.");

            // ── EmpresaId ─────────────────────────────────────────────────
            RuleFor(u => u.EmpresaId)
                .GreaterThan(0).WithMessage("Usuário deve estar vinculado a uma empresa.");

            // ── Cargo ─────────────────────────────────────────
            RuleFor(u => u.Cargo)
                .IsInEnum().WithMessage("Cargo inválido.");
        }
    }
}
