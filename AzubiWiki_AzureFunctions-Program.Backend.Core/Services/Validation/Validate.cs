using AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;

namespace AzubiWiki_AzureFunctions_Program.Backend.Core.Services.Validation
{
    public class Validate
    {
        public bool Car(CarQ car)
        {
            using var _ = new AssertionScope();

            car.Should().NotBeNull();

            car.Manufacturer.Should().NotBeNullOrWhiteSpace();
            car.Manufacturer = char.ToUpper(car.Manufacturer[0]) + car.Manufacturer.Substring(1).ToLower();
            car.Manufacturer.Should().NotContainAny("1","2","3","4","5","6","7","8","9","0","/","|","\\","'","\"","@",".","!","#","$","%","^","&","*","(",")","=","+","-").And.BeOneOf("Audi", "Tesla", "Ford", "Opel", "Volkswagen", "Mazda", "Bmw", "Mercedes", "Porsche", "Dacia", "Jeep", "Chevrolet", "Kia");


            car.Model.Should().NotBeNullOrWhiteSpace();

            
            car.Year.Should().NotBeNullOrWhiteSpace();
            int number;
            if(!int.TryParse(car.Year, out number))
            {
                throw new UnprocessableNumberValueException(nameof(car.Year));
            }

            number.Should().BeInRange(1749, 2024);

            car.Horsepower.Should().NotBeNullOrWhiteSpace();
            if(!int.TryParse(car.Horsepower, out number))
            {
                throw new UnprocessableNumberValueException(nameof(car.Horsepower));
            }
            number.Should().BeGreaterThanOrEqualTo(5);


            return true;
        }


        public bool NewCar(CarQ car)
        {
            if (!String.IsNullOrWhiteSpace(car.Manufacturer))
            {
                car.Manufacturer.Should().NotContainAny("1", "2", "3", "4", "5", "6", "7", "8", "9", "0").And.BeOneOf("Audi", "Opel", "Volkswagen", "Mazda", "BMW", "Mercedes", "Porsche", "Dacia", "Jeep", "Chevrolet", "KIA");
            }

            if (!String.IsNullOrWhiteSpace(car.Year))
            {
                int number;
                if (!int.TryParse(car.Year, out number))
                {
                    throw new UnprocessableNumberValueException(nameof(car.Year));
                }
                number.Should().BeInRange(1749, 2024);
            }

            if (!String.IsNullOrWhiteSpace(car.Horsepower))
            {
                int number;
                if(!int.TryParse(car.Horsepower, out number))
                {
                    throw new UnprocessableNumberValueException(nameof(car.Horsepower));
                }
                number.Should().BeGreaterThanOrEqualTo(5);
            }

            return true;
        }



        public bool Garage(GarageQ garage)
        {
            garage.BelongsTo.Should().NotBeNullOrWhiteSpace();

            return true;
        }
    }
}
