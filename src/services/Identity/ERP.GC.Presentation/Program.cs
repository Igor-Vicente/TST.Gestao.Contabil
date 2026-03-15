using ERP.GC.Presentation.Configuration;

namespace ERP.GC.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddIdentityConfig(builder.Configuration);
            builder.Services.AddAuthenticationConfig(builder.Configuration);
            builder.Services.AddAuthorizationConfig();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDependencies();
            builder.Services.AddSwaggerConfig();

            var app = builder.Build();

            app.UseMiddleware<MiddlewareException>();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
