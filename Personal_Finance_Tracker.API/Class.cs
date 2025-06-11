using Core.entities;
using Microsoft.AspNetCore.Identity;

namespace Personal_Finance_Tracker.API
{
    public static class DataSeeder
    {
        public static async Task SeedUsers(UserManager<Users_App> userManager)
        {
            const string baseUserName = "dummyuser";
            const string baseEmail = "dummy@example.com";
            const string defaultPassword = "Dummy@123";

            for (int i = 2; i <= 4; i++)
            {
                string userName = $"{baseUserName}{i}";
                string email = $"{baseEmail}{i}";

                var existingUser = await userManager.FindByEmailAsync(email);
                if (existingUser == null)
                {
                    var user = new Users_App
                    {
                        UserName = userName,
                        Email = email,
                        Created_At = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(user, defaultPassword);

                    if (result.Succeeded)
                    {
                        Console.WriteLine($"User '{userName}' added successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to add user '{userName}': " +
                                          string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    Console.WriteLine($"User '{userName}' already exists.");
                }
            }
        }
        /*public static async Task SeedUsers(UserManager<Users_App> userManager)
        {
            if (await userManager.FindByEmailAsync("dummy@example.com") == null)
            {
                var user = new Users_App
                {
                    Id = GlobalConstants.defaultID,
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
        */
    }
    public static class GlobalConstants
    {
        public const string defaultID = "aeaa657c-f669-4c8e-b470-333320a87f3c";
    }

}
