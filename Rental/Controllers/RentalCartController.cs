using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rental.Data.interfaces;
using Rental.Data.Models;
using Rental.ViewModels;

namespace Rental.Controllers
{
    [Authorize]
    public class RentalCartController : Controller
    {
        private readonly IAllCars _carRep;
        private readonly RentalCart _rentalCart;
        public RentalCartController(IAllCars carRep, RentalCart rentalCart)
        {
            _carRep = carRep;
            _rentalCart = rentalCart;

        }

        public ViewResult Index()
        {
            var items = _rentalCart.getRentalItems();
            _rentalCart.listRentalItems = items;

            var obj = new RentalCartViewModel
            {
                rentalCart = _rentalCart,
                TotalPrice = _rentalCart.GetTotalPrice() 
            };

            return View(obj);
        }



        public RedirectToActionResult addToCart(int id)
        {
            var item = _carRep.Cars.FirstOrDefault(i => i.id == id);
            if (item != null)
            {
               
                if (_rentalCart.listRentalItems.Any(i => i.car.id == id))
                {
                    TempData["Message"] = "Ви вже обрали даний Човен";
                }
                else
                {
                    _rentalCart.AddToCart(item);
                    TempData["Message"] = "Човен додано до кошика";
                }
            }

           
            return RedirectToAction("Index", "CarDetails", new { id = id });
        }



        [HttpPost]
        public RedirectToActionResult removeFromCart(int id)
        {
            _rentalCart.RemoveFromCart(id);
            return RedirectToAction("Index");
        }
    }
}