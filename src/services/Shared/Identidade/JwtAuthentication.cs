using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Shared.Identidade
{
    public static class JwtAuthentication
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            services.Configure<JwtSettings>(jwtSettings);

            var jwt = jwtSettings.Get<JwtSettings>()
                 ?? throw new InvalidOperationException("JwtSettings not defined in 'appsettings.json'");

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

            return services;
        }
    }
}
