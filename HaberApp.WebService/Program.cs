using HaberApp.Core.Helpers;
using HaberApp.DataAccess.Configurations;
using HaberApp.Services.ServiceConfigurations;
using HaberApp.WebService.Configurations;
using HaberApp.WebService.CustomAttributes;
using HaberApp.WebService.CustomMiddlewares;
using HaberApp.WebService.Jobs;
using HaberApp.WebService.Middlewares;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions((opt) =>
{
    opt.SuppressModelStateInvalidFilter = true;

}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.RegisterDbContext(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterIdentity();
builder.Services.RegisterAuth(builder.Configuration);
builder.Services.RegisterAutoMapperService();
builder.Services.RegisterServices();
builder.Services.RegisterCloudFlareSettings(builder.Configuration);
builder.Services.RegisterValidators();
builder.Services.RegisterSwagger();

var sqlConnection = builder.Configuration.GetConnectionString("SqlCon")
    ?? throw new InvalidOperationException("SqlCon bağlantı dizesi tanımlı değil.");

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(sqlConnection, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));
builder.Services.AddHangfireServer();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(CustomFilterAttribute<,>));


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.

ContextHelpers.SetContext(app.Services);
ServiceProviderHelper.InitServiceProvider(app.Services.GetRequiredService<IHttpContextAccessor>());



await app.SeedIdentityAsync();

var articleImportCron = builder.Configuration["Hangfire:ArticleImportCron"]
    ?? throw new InvalidOperationException("Hangfire:ArticleImportCron tanımlı değil.");
var istanbulTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
    OperatingSystem.IsWindows() ? "Turkey Standard Time" : "Europe/Istanbul");
app.Services.GetRequiredService<IRecurringJobManager>().AddOrUpdate<ArticleImportJob>(
    "daily-article-import",
    job => job.RunAsync(CancellationToken.None),
    articleImportCron,
    new RecurringJobOptions { TimeZone = istanbulTimeZone });


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

var dashboardUsername = builder.Configuration["Hangfire:Dashboard:Username"];
var dashboardPassword = builder.Configuration["Hangfire:Dashboard:Password"];
if (string.IsNullOrWhiteSpace(dashboardUsername) || string.IsNullOrWhiteSpace(dashboardPassword))
{
    app.Logger.LogWarning("Hangfire paneli kapalı. Kullanıcı adı ve parola yapılandırılmadı.");
}
else
{
    app.UseWhen(
        context => context.Request.Path.StartsWithSegments("/hangfire", StringComparison.OrdinalIgnoreCase),
        branch => branch.UseMiddleware<HangfireDashboardBasicAuthMiddleware>(dashboardUsername, dashboardPassword));

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = Array.Empty<IDashboardAuthorizationFilter>()
    });
}

app.MapControllers();

app.UseMiddleware<CustomErrorMiddleware>();

app.Run();
