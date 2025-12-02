using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Rental.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace Rental.Data
{
    public class DBObjects
    {

        public static async Task Initial(AppDBContent content, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {

            const string adminEmail = "admin@gmail.com";
            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                var defaultUser = new User() { Email = adminEmail, UserName = adminEmail };
                await userManager.CreateAsync(defaultUser, "password");
                user = defaultUser;
            }
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }

            if (content.Category.Any())
            {
                return;
            }

            Category classic = new() { categoryName = "Дизель", desc = "Авто з двигунами внутрішнього згорання" };
            Category electro = new() { categoryName = "Електро", desc = "Сучасний вид траспорту" };
            content.Category.AddRange(classic, electro);

            if (!content.Car.Any())
            {
                content.AddRange(new List<Car>
                {
                   
                }
                );
            }

            content.SaveChanges();

        }
    }
}