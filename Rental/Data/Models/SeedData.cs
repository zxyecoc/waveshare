using Microsoft.EntityFrameworkCore;
using Rental.Data.Models;

namespace Rental.Data
{
    public static class SeedData
    {
        public static async Task Initialize(AppDBContent db)
        {
            // 1) Категорія за замовчуванням
            var cat = await db.Category.FirstOrDefaultAsync(c => c.categoryName == "Класика");
            if (cat == null)
            {
                cat = new Category { categoryName = "Класика", desc = "Базова категорія" };
                db.Category.Add(cat);
                await db.SaveChangesAsync();
                Console.WriteLine("✔ Створено категорію 'Класика'");
            }

            // 2) Список човнів + фото
            var boats = new (string Name, string Img, string Short, string Long, decimal Price, bool Avail, int Seats, string Length, int Hp, decimal Eng, string Speed, string Color, string Year, string[] Pics)[]
            {
                ("Azimut 39", "/img/azimut39_1.jpg", "Комфортний човен середнього класу", "Ідеальний для прогулянок по воді та коротких подорожей.", 15000, true, 8,  "12 м", 760, 5.8m, "35 вузлів", "Білий", "2012",
                    new[]{"/img/azimut39_1.jpg","/img/azimut39_2.jpg","/img/azimut39_3.jpg","/img/azimut39_4.jpg","/img/azimut39_5.jpg"}),

                ("Azimut 42", "/img/azimut42_1.jpg", "Розкішний моторний човен", "Комфорт та стиль на воді. Простора палуба, сучасна каюта.", 20000, true, 10, "13 м", 800, 6.0m, "38 вузлів", "Білий", "2014",
                    new[]{"/img/azimut42_1.jpg","/img/azimut42_2.jpg","/img/azimut42_3.jpg","/img/azimut42_4.jpg"}),

                ("Fairline 62", "/img/fairline62_1.jpg", "Преміум-клас човен", "Для тривалих подорожей і VIP-відпочинку на морі.", 30000, true, 12, "19 м", 1200, 9.5m, "45 вузлів", "Сірий", "2018",
                    new[]{"/img/fairline62_1.jpg","/img/fairline62_2.jpg","/img/fairline62_3.jpg","/img/fairline62_4.jpg","/img/fairline62_5.jpg"}),

                ("Relax", "/img/relax_1.jpg", "Легкий човен для прогулянок", "Для риболовлі або короткої прогулянки.", 5000, true, 5, "6 м", 150, 2.2m, "25 вузлів", "Синій", "2017",
                    new[]{"/img/relax_1.jpg","/img/relax_2.jpg","/img/relax_3.jpg","/img/relax_4.jpg"}),

                ("Sinergia", "/img/sinergia_1.jpg", "Сучасний човен середнього класу", "Збалансований човен для комфорту та швидкості.", 12000, true, 7, "9 м", 500, 4.0m, "33 вузли", "Біло-синій", "2019",
                    new[]{"/img/sinergia_1.jpg","/img/sinergia_2.jpg","/img/sinergia_3.jpg"}),
            };

            foreach (var b in boats)
            {
                // upsert човна за назвою
                var boat = await db.Car.Include(c => c.Images).FirstOrDefaultAsync(c => c.name == b.Name);
                if (boat == null)
                {
                    boat = new Car
                    {
                        name = b.Name,
                        shortDesc = b.Short,
                        longDesc = b.Long,
                        price = b.Price,
                        available = b.Avail,
                        seatingCapacity = b.Seats,
                        bodyType = b.Length,                 // довжина
                        horsepower = b.Hp,
                        engineDisplacement = b.Eng,
                        fuelType = b.Speed,
                        color = b.Color,
                        transmission = b.Year,            // рік випуску
                        img = b.Img,
                        CategoryId = cat.id
                    };
                    db.Car.Add(boat);
                    await db.SaveChangesAsync();
                    Console.WriteLine($"✔ Додано човен: {boat.name}");
                }

                // додати відсутні фото
                var existing = new HashSet<string>(boat.Images?.Select(i => i.Url.Replace('\\', '/')) ?? Enumerable.Empty<string>());
                foreach (var pic in b.Pics)
                {
                    var url = pic.Replace('\\', '/'); // нормалізуємо
                    if (!existing.Contains(url))
                    {
                        db.CarImages.Add(new CarImage { CarId = boat.id, Url = url });
                        Console.WriteLine($"   + фото: {url}");
                    }
                }
            }

            await db.SaveChangesAsync();
            Console.WriteLine("✅ SeedData: човни і фото синхронізовані.");
        }
    }
}
