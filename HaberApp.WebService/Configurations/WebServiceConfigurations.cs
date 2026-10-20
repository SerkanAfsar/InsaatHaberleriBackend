using HaberApp.Core.Domains;
using HaberApp.Core.Utils;
using HaberApp.DataAccess.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace HaberApp.WebService.Configurations
{
    public static class WebServiceConfigurations
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection RegisterSwagger()
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));

                services.AddSwaggerGen(c =>
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization"
                    });

                    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
                });
                return services;
            }


        }
        extension(WebApplication app)
        {
            public async Task SeedIdentityAsync()
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    await db.Database.MigrateAsync();
                }
                catch (Exception)
                {

                }

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

                const string adminEmail = "serkanafsar84@gmail.com";
                const string adminPassword = "1Q2w3E4r!";

                if (!await roleManager.RoleExistsAsync(RoleConstants.RootAdmin))
                {
                    await roleManager.CreateAsync(new AppRole()
                    {
                        Name = RoleConstants.RootAdmin
                    });
                }
                if (!await roleManager.RoleExistsAsync(RoleConstants.NormalUser))
                {
                    await roleManager.CreateAsync(new AppRole()
                    {
                        Name = RoleConstants.NormalUser
                    });
                }



                var user = await userManager.FindByEmailAsync(adminEmail);

                if (user == null)
                {
                    user = new AppUser
                    {
                        Email = adminEmail,
                        UserName = adminEmail,
                        EmailConfirmed = true,
                        Name = "Serkan",
                        Surname = "Afşar",


                    };

                    var deneme1Result = await userManager.CreateAsync(user, adminPassword);
                    var deneme2Result = await userManager.AddToRolesAsync(user, new List<string>()
                {
                   RoleConstants.RootAdmin,
                   RoleConstants.NormalUser,
                });

                }

                var denemeUser = new AppUser()
                {
                    Email = "mila@deneme.com",
                    UserName = "mila@deneme.com",
                    EmailConfirmed = true,
                    Name = "Mila",
                    Surname = "Yılmazer",

                };

                await userManager.CreateAsync(denemeUser, "1Q2w3E4r!");
                await userManager.AddToRolesAsync(denemeUser, new List<string>()
            {
               RoleConstants.NormalUser,
            });

            }
        }
    }
}
