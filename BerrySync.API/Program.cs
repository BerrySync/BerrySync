using BerrySync.Data;
using BerrySync.Updater;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureServices((hostingContext, services) =>
{
    services.AddDataContext();
    services.AddUpdater();

    services.AddControllers()
        .AddJsonOptions(o =>
        {
            o.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.Preserve;
        });
})
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });

var app = builder.Build();

app.SetupDb();
app.CreateDirStructure();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider("/data/calendars"),
    RequestPath = "/static/calendars"
});

app.UseAuthorization();
app.MapControllers();

app.Run();