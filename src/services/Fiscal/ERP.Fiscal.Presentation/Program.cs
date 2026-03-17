using ERP.Fiscal.Presentation.Configuration;
using Shared.Identidade;

namespace ERP.Fiscal.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddJwtSwaggerGen();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddDependencies(builder.Configuration);

            var app = builder.Build();

            app.UseJwtSwagger();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
