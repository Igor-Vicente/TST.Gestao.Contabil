using ERP.Registro.Presentation.Abstractions;
using ERP.Registro.Presentation.Commands;
using ERP.Registro.Presentation.Commands.Handlers;
using ERP.Registro.Presentation.Data;
using ERP.Registro.Presentation.Data.Repositories;
using ERP.Registro.Presentation.Models;
using ERP.Registro.Presentation.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Identidade;

namespace ERP.Registro.Presentation.Configuration
{
    public static class StartupExtentions
    {
        public static void AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SQLConnection")));

            services.AddIdentity<Usuario, IdentityRole<int>>()
                .AddEntityFrameworkStores<AuthDbContext>();
        }

        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthUser, AuthUser>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IRequestHandler<AdicionarEmpresaCommand, Empresa>, EmpresaCommandHandler>();

            services.AddScoped<IMediator, Mediator>();
            services.AddScoped<INotificador, Notificador>();

            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        }

        /// <summary>
        /// Configura políticas de autorização baseadas em Cargo (AdministradorGeral, Gestor, Colaborador).
        /// - PodeCriarEmpresa: apenas AdministradorGeral.
        /// - PodeCriarUsuario: AdministradorGeral ou Gestor (Gestor só pode criar usuários da própria empresa; validar no controller).
        /// </summary>
        public static void AddAuthorizationConfig(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("PodeCriarEmpresa", policy =>
                    policy.AddRequirements(new CargoRequirement(Cargo.AdministradorGeral)));

                options.AddPolicy("PodeCriarUsuario", policy =>
                    policy.AddRequirements(new CargoRequirement(Cargo.AdministradorGeral, Cargo.Gestor)));
            });

            services.AddScoped<IAuthorizationHandler, CargoAuthorizationHandler>();
        }
    }
}
