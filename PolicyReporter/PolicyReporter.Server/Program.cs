using PolicyReporter.DataHandling;
using PolicyReporter.Server;
using PolicyReporter.Server.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddControllers();
services.AddOpenApi();
services.AddScoped<IPolicyRepository>(_ => new PolicyRepository(new PolicyDbContext()));
services.AddScoped<IPolicyReportService, PolicyReportService>();
services.AddAutoMapper(typeof(MappingProfile));

WebApplication app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization(); // TODO implement fully

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
