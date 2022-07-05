using BerrySync.Data;
using BerrySync.Updater;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureServices((hostingContext, services) =>
{
    services.AddDataContext(hostingContext.Configuration);
    services.AddUpdater(hostingContext.Configuration);

    services.AddControllers();
})
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });

var app = builder.Build();

app.SetupDb();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();