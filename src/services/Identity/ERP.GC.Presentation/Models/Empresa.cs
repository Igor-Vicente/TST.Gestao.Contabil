using FluentValidation;

namespace ERP.GC.Presentation.Models
{
    public enum RegimeTributario
    {
        SimplesNacional = 1,
        LucroPresumido = 2,
        LucroReal = 3
    }

    public class Empresa : Entity
    {
        public string Cnpj { get; private set; }
        public string RazaoSocial { get; private set; }
        public string NomeFantasia { get; private set; }
        public RegimeTributario RegimeTributario { get; private set; }
        public string Email { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime CriadoEm { get; private set; }

        // Navegação
        public ICollection<Usuario> Usuarios { get; set; }

        public Empresa(string cnpj, string razaoSocial, string nomeFantasia, RegimeTributario regimeTributario, string email, bool ativo, DateTime criadoEm)
        {
            Cnpj = cnpj;
            RazaoSocial = razaoSocial;
            NomeFantasia = nomeFantasia;
            RegimeTributario = regimeTributario;
            Email = email;
            Ativo = ativo;
            CriadoEm = criadoEm;
        }

        public override bool IsValid()
        {
            ValidationResult = new EmpresaValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class EmpresaValidator : AbstractValidator<Empresa>
    {
        public EmpresaValidator()
        {
            // ── CNPJ ────────────────────────────────────────────────────
            RuleFor(e => e.Cnpj)
                .NotEmpty().WithMessage("CNPJ é obrigatório.")
                .Length(14).WithMessage("CNPJ deve conter exatamente 14 dígitos.")
                .Matches(@"^\d{14}$").WithMessage("CNPJ deve conter apenas números, sem pontos ou traços.");

            // ── Razão Social ─────────────────────────────────────────────
            RuleFor(e => e.RazaoSocial)
                .NotEmpty().WithMessage("Razão Social é obrigatória.")
                .MaximumLength(150).WithMessage("Razão Social deve ter no máximo 150 caracteres.");

            // ── Nome Fantasia (opcional) ──────────────────────────────────
            RuleFor(e => e.NomeFantasia)
                .MaximumLength(150).WithMessage("Nome Fantasia deve ter no máximo 150 caracteres.")
                .When(e => e.NomeFantasia is not null);

            // ── Regime Tributário ─────────────────────────────────────────
            RuleFor(e => e.RegimeTributario)
                .IsInEnum().WithMessage("Regime Tributário inválido.");

            // ── Email ─────────────────────────────────────────────────────
            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório.")
                .MaximumLength(150).WithMessage("E-mail deve ter no máximo 150 caracteres.")
                .EmailAddress().WithMessage("E-mail inválido.");
        }
    }
}
