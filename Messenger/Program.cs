using FluentValidation;
using FluentValidation.AspNetCore;
using Messenger;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
ConfigurationBase(builder);
ConfigurationMetrics(builder);
ConfigurationValidation(builder);
ConfigurationCustomServices(builder);
ConfigurationAuthorization(builder);

void ConfigurationValidation(WebApplicationBuilder builder)
{
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<RegistrationUserValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<AuthorizationUserValidator>();
}

void ConfigurationMetrics(WebApplicationBuilder builder)
{
    builder.Services.AddHealthChecks().ForwardToPrometheus();
    builder.Services.AddSingleton<IMetricsService, MetricsService>();
}

void ConfigurationCustomServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
    builder.Services.AddScoped<IMessageRepository, MessageRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserValidationService, UserValidationService>();
    builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
    builder.Services.AddScoped<IChatService, ChatService>();
    builder.Services.AddScoped<IClaimService, ClaimService>();
    builder.Services.AddScoped<IMessagingService, MessagingService>();
}

void ConfigurationAuthorization(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication("Cookies").AddCookie(options => options.LoginPath = "/Authorization");
    builder.Services.AddAuthorization();
}

void ConfigurationBase(WebApplicationBuilder builder)
{
    builder.Services.AddControllersWithViews();
    builder.Services.AddSignalR();
    builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddHttpContextAccessor();

    builder.Configuration.Sources
        .OfType<FileConfigurationSource>()
        .ToList()
        .ForEach(source => source.ReloadOnChange = false);
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpMetrics();

app.MapStaticAssets();

app.MapControllers();

app.MapMetrics("/metrics");

app.MapHub<ChatHub>("chatHub");

app.MapGet("/", () => Results.Redirect("/MainPage"));

app.MapHealthChecks("/health");

app.Run();
