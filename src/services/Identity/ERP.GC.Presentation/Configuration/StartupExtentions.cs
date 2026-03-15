using ERP.GC.Presentation.Abstractions;
using ERP.GC.Presentation.Commands;
using ERP.GC.Presentation.Commands.Handlers;
using ERP.GC.Presentation.Data;
using ERP.GC.Presentation.Data.Repositories;
using ERP.GC.Presentation.Models;
using ERP.GC.Presentation.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ERP.GC.Presentation.Configuration
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
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IRequestHandler<AdicionarEmpresaCommand, Empresa>, EmpresaCommandHandler>();

            services.AddScoped<IMediator, Mediator>();
            services.AddScoped<INotificador, Notificador>();

            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        }

        public static void AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration)
        {
            /* Obtém a seção "JwtConfigSettings" do appsettings.json */
            var jwtSettings = configuration.GetSection("JwtConfigSettings");

            /* Diz ao sistema de injeção de dependência para carregar as configurações da seção "JwtConfigSettings" no objeto JwtConfigSettings */
            services.Configure<JwtConfigSettings>(jwtSettings);

            /* Converte a seção de configuração em um objeto concreto. */
            var jwt = jwtSettings.Get<JwtConfigSettings>()
                ?? throw new InvalidOperationException("JwtConfigSettings not defined in 'app settings'");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                /* Garante que apenas conexões HTTPS possam enviar tokens (por segurança). */
                options.RequireHttpsMetadata = true;

                /* Faz com que o token JWT seja armazenado no HttpContext após a autenticação, caso você precise acessá-lo no backend. */
                options.SaveToken = true;

                /* Define as regras de validação do token */
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    RequireExpirationTime = true,
                    RequireSignedTokens = true,

                    ClockSkew = TimeSpan.Zero,

                    ValidAudience = jwt.Audience,
                    ValidIssuer = jwt.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwt.SecretKey)), // Define a chave secreta usada para validar a assinatura do token.
                };
            });
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

        public static void AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(setup =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });
        }
    }
}
