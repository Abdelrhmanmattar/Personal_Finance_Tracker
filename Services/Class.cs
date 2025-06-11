using Core.entities;
using Microsoft.AspNetCore.Identity;

namespace Services
{
    public static class DataSeeder
    {
        public static async Task SeedUsers(UserManager<Users_App> userManager)
        {
            if (await userManager.FindByEmailAsync("dummy@example.com") == null)
            {
                var user = new Users_App
                {
                    UserName = "dummyuser",
                    Email = "dummy@example.com",
                    Created_At = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, "Dummy@123");

                if (result.Succeeded)
                {
                    Console.WriteLine("Dummy user added successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to add dummy user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
    public static class GlobalConstants
    {
        public const string defaultID = "aeaa657c-f669-4c8e-b470-333320a87f3c";
    }

}
