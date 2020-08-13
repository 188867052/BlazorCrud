using BlazorCrud.Shared.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace BlazorCrud.Server
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                MyExtentions.Elasticsearch.ConfigureSerilog(nameof(BlazorCrud));
                Log.Information("Starting host...");

                var host = CreateWebHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        var patientContext = services.GetRequiredService<PatientContext>();
                        var organizationContext = services.GetRequiredService<OrganizationContext>();
                        var claimContext = services.GetRequiredService<ClaimContext>();
                        var uploadContext = services.GetRequiredService<UploadContext>();
                        var userContext = services.GetRequiredService<UserContext>();
                        DataInitializer.Initialize(patientContext, organizationContext, claimContext, uploadContext, userContext);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred creating the DB.");
                    }
                }
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
