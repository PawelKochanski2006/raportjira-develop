using JiraRaporty.Domain.Interfaces;
using JiraRaporty.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace JiraRaporty.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IExcelService, ExcelService>();

            services.AddScoped<IJiraApiService>(provider =>
            {
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                var session = httpContextAccessor.HttpContext?.Session;

                if (session == null)
                {
                    throw new Exception("HTTP context or session is not available");
                }

                var username = session.GetString("_Name");
                var password = session.GetString("_Pass");
                var jiraUrl = configuration["JiraSettings:ApiUrl"];

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(jiraUrl))
                {
                    throw new Exception("User credentials or Jira URL not found");
                }

                return new JiraApiService(jiraUrl, username, password);
            });
        }
    }
}