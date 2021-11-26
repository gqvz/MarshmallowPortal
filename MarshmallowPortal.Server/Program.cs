using Google.OAuth2;
using MarshmallowPortal.Server.Data;
using MarshmallowPortal.Server.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("log.txt")
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "MarshmallowPortal.Server", Version = "v1"}); });
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite("Data Source=Db/app.db"));
builder.Configuration.AddJsonFile("Config.json");
builder.Services.AddSingleton(_ =>
{
    var options = builder.Configuration.Get<GoogleOptions>();
    var credentials = new GoogleCredentials(options.ClientId, options.ClientSecret);
    return credentials;
});
builder.Services.AddSingleton(provider =>
{
    var credentials = provider.GetService<GoogleCredentials>();
    if (credentials == null) throw new Exception("No");
    var service = new OAuth2Service
    {
        Credentials = new ApplicationCredentials(
            credentials, new[] {OAuth2Service.Profile}, "user", "MarshmallowPortal.Server")
    };
    return service;
});
var app = builder.Build();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MarshmallowPortal.Server v1"));
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();