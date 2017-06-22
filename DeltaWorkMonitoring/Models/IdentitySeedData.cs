using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeltaWorkMonitoring.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminEmail = "admin@example.com";
        private const string adminPassword = "Secret123$";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            UserManager<AppUser> userManager = app.ApplicationServices
                .GetRequiredService<UserManager<AppUser>>();

            AppUser user = await userManager.FindByIdAsync(adminUser);
            if (user == null)
            {
                user = new AppUser();
                user.UserName = adminUser;
                user.Email = adminEmail;
                await userManager.CreateAsync(user, adminPassword);
            }
        }
    }
}
