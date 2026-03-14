using FluentValidation;

namespace ERP.GC.Presentation.Models
{
    public class Usuario : Entity
    {
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public int EmpresaId { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime CriadoEm { get; private set; }


        // Navegação
        public Empresa Empresa { get; set; }

        public Usuario(int id, string nome, string email, int empresaId, bool ativo, DateTime criadoEm)
        {
            Id = id;
            Nome = nome;
            Email = email;
            EmpresaId = empresaId;
            Ativo = ativo;
            CriadoEm = criadoEm;
        }

        public override bool IsValid()
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
        }
    }
}
