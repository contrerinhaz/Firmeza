namespace Firmeza.Web.Data
{
    using Microsoft.AspNetCore.Identity;
    using Models.Entities;

    public static class SeedData
    {
       public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger("SeedData");

            // Create roles
            string[] roles = { "Admin", "Client" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger?.LogInformation($"Role '{role}' created successfully.");
                }
            }

            // Create admin user
            string adminEmail = "admin@gmail.com";
            string adminPassword = "admin123.";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator",
                    DocumentNumber = "0000000000",
                    Phone = "000000000",
                    RegisterDate = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger?.LogInformation("✅ Admin user created successfully.");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                        logger?.LogError($"⚠️ Error creating admin user: {error.Description}");
                }
            }
            else
            {
                logger?.LogInformation("ℹ️ Admin user already exists. No action taken.");
            }
        }
    }
}
