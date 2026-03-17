using ERP.Fiscal.Presentation.Services;
using ERP.Fiscal.Presentation.Services.Handlers;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Shared.Identidade;

namespace ERP.Fiscal.Presentation.Configuration
{
    public static class StartupExtentions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthUser, AuthUser>();

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IRegistroService, RegistroService>(client => { client.BaseAddress = new Uri(configuration.GetValue<string>("registroUrl") ?? ""); })
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()  //adiciona o token de autorização do usuário logado em todas as requisições feitas por esse HttpClient    
                .AddPolicyHandler(GetRetryPolicy())  //tenta novamente a requisição + 3 vezes aguardando 5 segundos entre cada tentativa
                    .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));  //após 5 falhas consecutivas de requisição, impede a comunição na api por 30 seg

            return services;
        }

        private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var retry = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(5),
                (outcome, timespan, retryCount, context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Tentando pela {retryCount + 1} vez!");
                    Console.ForegroundColor = ConsoleColor.White;
                });
            return retry;
        }
    }
}