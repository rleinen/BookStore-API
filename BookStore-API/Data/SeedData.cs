using Microsoft.AspNetCore.Identity;

namespace BookStore_API.Data
{
    public static class SeedData
    {
        public async static Task Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }

        private async static Task SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@bookstore.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@bookstore.com"
                };
                var result = await userManager.CreateAsync(user, "Rkl1433!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "admin");
                }
            }

            if (await userManager.FindByEmailAsync("customer@yahoo.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer",
                    Email = "customer@yahoo.com"
                };
                var result = await userManager.CreateAsync(user, "Rkl1433!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "customer");
                }
            }

            if (await userManager.FindByEmailAsync("customer2@yahoo.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer2",
                    Email = "customer2@yahoo.com"
                };
                var result = await userManager.CreateAsync(user, "Rkl1433!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "customer");
                }
            }


        }

        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (! await roleManager.RoleExistsAsync("admin"))
            {
                var role = new IdentityRole
                {
                    Name = "admin"
                };
                var result = await roleManager.CreateAsync(role);

            }

            if (!await roleManager.RoleExistsAsync("customer"))
            {
                var role = new IdentityRole
                {
                    Name = "customer"
                };
                var result = await roleManager.CreateAsync(role);

            }

        }

    }
}
