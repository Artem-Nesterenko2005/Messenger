using FluentValidation;
using FluentValidation.AspNetCore;
using Messenger;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(8080); // Стандартный порт для облаков
    });
}

// Настройка HTTPS редиректа
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443;
});

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddAuthentication("Cookies").AddCookie(options => options.LoginPath = "/Authorization");
builder.Services.AddAuthorization();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<RegistrationUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AuthorizationUserValidator>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserValidationService, UserValidationService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IChatService, ChatService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapHub<ChatHub>("chatHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=MainPage}/{id?}");

app.Run();
