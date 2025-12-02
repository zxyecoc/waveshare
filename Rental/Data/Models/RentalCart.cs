using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rental.Data.Models
{
    public class RentalCart
    {
        private readonly AppDBContent appDBContent;

        public RentalCart(AppDBContent appDBContent)
        {
            this.appDBContent = appDBContent;
            listRentalItems = new List<RentalCartItem>(); // Ініціалізація порожнього списку
        }

        public string RentalCartId { get; set; }
        public List<RentalCartItem> listRentalItems { get; set; }

        public static RentalCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<AppDBContent>();
            string rentalCartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", rentalCartId);

            return new RentalCart(context) { RentalCartId = rentalCartId };
        }

        public decimal GetTotalPrice()
        {
            return listRentalItems.Sum(item => item.price);
        }

        public void AddToCart(Car car)
        {
            // Завантажуємо поточний список елементів кошика, щоб уникнути проблеми з null
            listRentalItems = getRentalItems();

            // Перевірка, чи Човен вже в кошику
            var existingItem = listRentalItems.FirstOrDefault(i => i.car.id == car.id);
            if (existingItem == null)
            {
                appDBContent.RentalCartItem.Add(new RentalCartItem
                {
                    RentalCartId = RentalCartId,
                    car = car,
                    price = car.price
                });

                appDBContent.SaveChanges();
            }
            else
            {
                // Додайте повідомлення TempData про те, що авто вже в кошику
                // TempData["Message"] = "Авто вже додано до кошика";
            }
        }

        public void RemoveFromCart(int id)
        {
            var item = appDBContent.RentalCartItem.FirstOrDefault(r => r.id == id);
            if (item != null)
            {
                appDBContent.RentalCartItem.Remove(item);
                appDBContent.SaveChanges();
            }
        }

        public void ClearCart()
        {
            var items = appDBContent.RentalCartItem.Where(c => c.RentalCartId == RentalCartId);
            appDBContent.RentalCartItem.RemoveRange(items);
            appDBContent.SaveChanges();
        }

        public List<RentalCartItem> getRentalItems()
        {
            return appDBContent.RentalCartItem
                .Where(c => c.RentalCartId == RentalCartId)
                .Include(s => s.car)
                .ToList();
        }
    }
}
