using System;
using MarshmallowPortal.OAuth2.Discord;
using MarshmallowPortal.OAuth2.Github;
using MarshmallowPortal.OAuth2.Google;
using MarshmallowPortal.Server.Data;
using MarshmallowPortal.Server.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "MarshmallowPortal.Server", Version = "v1"}); });

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("Config.json")
    .AddCommandLine(args);

builder.Services.AddSingleton(_ =>
{
    var options = builder.Configuration.GetSection("GoogleOptions").Get<GoogleOptions>();
    var credentials = new GoogleCredentials(options.ClientId, options.ClientSecret);
    return credentials;
});

builder.Services.AddSingleton(_ =>
{
    var options = builder.Configuration.GetSection("DiscordOptions").Get<DiscordOptions>();
    var credentials = new DiscordCredentials(options.ClientId, options.ClientSecret);
    return credentials;
});

builder.Services.AddSingleton(_ =>
{
    var options = builder.Configuration.GetSection("GithubOptions").Get<GithubOptions>();
    var credentials = new GithubCredentials(options.ClientId, options.ClientSecret);
    return credentials;
});

builder.Services.AddSingleton(provider =>
{
    var credentials = provider.GetService<GoogleCredentials>();
    if (credentials == null) throw new Exception("No");
    var service = new GoogleOAuth2Service(credentials, new[] {GoogleOAuth2Service.Profile, GoogleOAuth2Service.Email});
    return service;
});

builder.Services.AddSingleton(provider =>
{
    var credentials = provider.GetService<DiscordCredentials>();
    if (credentials == null) throw new Exception("No");
    var service = new DiscordOAuth2Service(credentials, new[] {DiscordOAuth2Service.Identify, DiscordOAuth2Service.Email});
    return service;
});

builder.Services.AddSingleton(provider =>
{
    var credentials = provider.GetService<GithubCredentials>();
    if (credentials == null) throw new Exception("No");
    var service = new GithubOAuth2Service(credentials, new[] {GithubOAuth2Service.User});
    return service;
});

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite("Data Source=Db/app.db"));

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMvc();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MarshmallowPortal.Server v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();