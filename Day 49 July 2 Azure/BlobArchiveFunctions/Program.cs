using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlobArchiveFunctions;
 
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication() // <-- This is the line that changed!
    .ConfigureServices(services =>
    {
        services.AddTransient<BlobArchiveService>();
    })
    .Build();
 
host.Run();