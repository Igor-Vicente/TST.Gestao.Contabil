using ERP.GC.Presentation.Models;
using Microsoft.AspNetCore.Authorization;

namespace ERP.GC.Presentation.Configuration
{
    /// <summary>
    /// Requisito de autorização baseado no cargo do usuário (claim "Cargo").
    /// </summary>
    public class CargoRequirement : IAuthorizationRequirement
    {
        public IReadOnlyList<Cargo> CargosPermitidos { get; }

        public CargoRequirement(params Cargo[] cargosPermitidos)
        {
            CargosPermitidos = cargosPermitidos?.ToList() ?? new List<Cargo>();
        }
    }

    public class CargoAuthorizationHandler : AuthorizationHandler<CargoRequirement>
    {
        private const string CargoClaimType = "Cargo";

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CargoRequirement requirement)
        {
            if (requirement.CargosPermitidos.Count == 0)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var cargoClaim = context.User.FindFirst(CargoClaimType)?.Value;
            if (string.IsNullOrEmpty(cargoClaim))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (Enum.TryParse<Cargo>(cargoClaim, ignoreCase: true, out var cargo) &&
                requirement.CargosPermitidos.Contains(cargo))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
