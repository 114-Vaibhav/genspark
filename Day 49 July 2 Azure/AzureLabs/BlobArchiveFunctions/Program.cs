// using BlobArchiveFunctions;
// using Microsoft.Azure.Functions.Worker.Builder;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;

// var builder = FunctionsApplication.CreateBuilder(args);

// builder.ConfigureFunctionsWebApplication();

// builder.Services.AddTransient<BlobArchiveService>();

// builder.Build().Run();
using BlobArchiveFunctions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddTransient<BlobArchiveService>();
    })
    .Build();

host.Run();