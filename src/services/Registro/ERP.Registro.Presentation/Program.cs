using ERP.Registro.Presentation.Configuration;
using Shared.Identidade;

namespace ERP.Registro.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddIdentityConfig(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorizationConfig();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDependencies();
            builder.Services.AddJwtSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<MiddlewareException>();
            app.UseJwtSwagger();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
