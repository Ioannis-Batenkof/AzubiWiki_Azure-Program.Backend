using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Services;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Services.Mapping;
using AzubiWiki_AzureFunctions_Program.Backend.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton<IStorageService<Car>, CarRepository>();
        services.AddSingleton<IStorageService<Garage>, GarageRepository>();
        services.AddSingleton<IBackupService, BackupRepository>();

        services.AddSingleton<IFileHandler, FileHandler>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo());
        });

        services.AddAutoMapper(
            cfg => cfg.AddProfile<MappingProfile>(),
            AppDomain.CurrentDomain.GetAssemblies());
    })
    .Build();

host.Run();
