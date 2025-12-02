using Rental.Data.interfaces;
using Rental.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Rental.Data.mocks
{
    public class MockCars : IAllCars
    {
        private readonly ICarsCategory _categoryCars;

        public MockCars()
        {
            _categoryCars = new MockCategory();
        }

        

        public IEnumerable<Car> Cars
        {
            get
            {
                return new List<Car>
                {
                   
                };
            }
        }

        public IEnumerable<Car> getFavCars => Cars.Where(c => c.isFavourite);

        public Car getObjectCar(int carId)
        {
            return Cars.FirstOrDefault(c => c.id == carId);
        }

        public Car GetCar(int id)
        {
            return Cars.FirstOrDefault(c => c.id == id);
        }
    }
}
