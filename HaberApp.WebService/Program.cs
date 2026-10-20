using HaberApp.Core.Helpers;
using HaberApp.DataAccess.Configurations;
using HaberApp.Services.ServiceConfigurations;
using HaberApp.WebService.Configurations;
using HaberApp.WebService.CustomAttributes;
using HaberApp.WebService.CustomMiddlewares;

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


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<CustomErrorMiddleware>();

app.Run();
