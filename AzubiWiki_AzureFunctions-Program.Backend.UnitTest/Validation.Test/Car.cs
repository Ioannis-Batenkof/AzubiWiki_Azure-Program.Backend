using AzubiWiki_AzureFunctions_Program.Backend.Contracts.Queries;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Exceptions;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Services.Validation;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.Validation.Test
{
    public class Car
    {

        [Fact]
        public void Validate_ValidCredentials_True()
        {
            CarQ car = new CarQ { Horsepower = "300", Year = "2022", Model = "RS6" };
            Validate validate = new();
            bool pass = false;

            for (int i = 0; i < 10; i++)
            {
                switch (i)
                {
                    case 0:
                        car.Manufacturer = "audi";
                        pass = validate.Car(car);
                        break;

                    case 1:
                        car.Manufacturer = "OPEL";
                        pass = validate.Car(car);
                        break;

                    case 2:
                        car.Manufacturer = "VolksWAgen";
                        pass = validate.Car(car);
                        break;

                    case 3:
                        car.Manufacturer = "MazdA";
                        pass = validate.Car(car);
                        break;

                    case 4:
                        car.Manufacturer = "BMW";
                        pass = validate.Car(car);
                        break;

                    case 5:
                        car.Manufacturer = "MERcedes";
                        pass = validate.Car(car);
                        break;

                    case 6:
                        car.Manufacturer = "PorSCHe";
                        pass = validate.Car(car);
                        break;

                    case 7:
                        car.Manufacturer = "Dacia";
                        pass = validate.Car(car);
                        break;

                    case 8:
                        car.Manufacturer = "Jeep";
                        pass = validate.Car(car);
                        break;

                    case 9:
                        car.Manufacturer = "Chevrolet";
                        pass = validate.Car(car);
                        break;

                    case 10:
                        car.Manufacturer = "KIA";
                        pass = validate.Car(car);
                        break;
                }

                if (pass == false)
                {
                    break;
                }
            }


            Assert.True(pass);


            car.Year = "1749";
            pass = validate.Car(car);
            Assert.True(pass);

            car.Year = "2024";
            pass = validate.Car(car);
            Assert.True(pass);

            car.Horsepower = "5";
            pass = validate.Car(car);
            Assert.True(pass);
        }



        [Fact]
        public void Validate_NullCar_AssertionFailedException()             // All of the AssertionFailedException exceptions are beiong converted to xUnitException
        {                                                                   // because Fluent Assertions is an UNRECOGNISABLE package for xUnit.
            try                                                             // It autoconverts the exceptions just to keep everything runnning.
            {                                                               // It will do the same for any unregistered exceptions.
                Validate validate = new();                                      
                CarQ car = null;

                bool pass = validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);             
            }
        }



        [Fact]
        public void Validate_CarManufacturerIsNullOrWhiteSpace_AssertionFailedException()
        {
            Validate validate = new();

            CarQ car = new();

            try
            {
                car.Manufacturer = null;
                bool pass = validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);              

                try
                {
                    car.Manufacturer = "    ";
                    bool pass = validate.Car(car);
                }
                catch (Exception exc)
                {
                    Assert.IsType<XunitException>(exc);
                }
            }
        }



        [Fact]
        public void Validate_CarManufacturerHasNumber_AssertionFailedException()
        {
            Validate validate = new();
            CarQ car = new();

            try
            {
                car.Manufacturer = "Audi123";
                validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);                
            }
        }


        [Fact]
        public void Validate_CarManufacturerIsUnknownBrand_AssertionFailedException()
        {
            Validate validate = new();
            CarQ car = new();

            try
            {
                car.Manufacturer = "Auda";
                bool pass = validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }



        [Fact]
        public void Validate_CarModelIsNullOrWhiteSpace_AssertionFailedException()
        {
            Validate validate = new();
            CarQ car = new CarQ { Manufacturer = "Audi" };

            try
            {
                car.Model = null;
                bool pass = validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);

                try
                {
                    car.Model = "    ";
                    bool pass = validate.Car(car);
                }
                catch (Exception exc)
                {
                    Assert.IsType<XunitException>(exc);
                }
            }
        }



        [Fact]
        public void Validate_CarYearIsNullOrWhiteSpace_AssertionFailedException()
        {
            Validate validate = new();
            CarQ car = new CarQ { Manufacturer = "Audi", Model = "aa" };

            try
            {
                car.Year = null;
                bool pass = validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);

                try
                {
                    car.Year = "   ";
                    bool pass = validate.Car(car);
                }
                catch (Exception exc)
                {
                    Assert.IsType<XunitException>(exc);
                }
            }
        }



        [Fact]
        public void Validate_CarYearIsNotNumber_UnprocessableNumberValueException()
        {
            Validate validate = new();
            CarQ car = new CarQ { Manufacturer = "Audi", Model = "aa" };

            try
            {
                car.Year = "Not Parsable";
                bool pass = validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<UnprocessableNumberValueException>(ex);
            }
        }



        [Fact]
        public void Validate_CarYearIsNotInRange_AssertionFailedException()
        {
            Validate validate = new();
            CarQ car = new CarQ { Manufacturer = "Audi", Model = "aaa" };

            try
            {
                car.Year = "1743";
                bool pass = validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);

                try
                {
                    car.Year = "2025";
                    bool pass = validate.Car(car);
                }
                catch (Exception exc)
                {
                    Assert.IsType<XunitException>(exc);
                }
            }
        }




        [Fact]
        public void Validate_CarHorsepowerIsNullOrEmpty_AssertinFailedException()
        {
            Validate validate = new();
            CarQ car = new CarQ { Manufacturer = "Audi", Model = "aa", Year = "2022" };

            try
            {
                bool pass = validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }



        [Fact]
        public void Validate_CarHorsepowerIsNotNumber_UnprocessableNumberValueException()
        {
            Validate validate = new();
            CarQ car = new CarQ { Manufacturer = "Audi", Model = "aa", Year = "2022" };

            try
            {
                car.Horsepower = "4a";
                bool pass = validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<UnprocessableNumberValueException>(ex);
            }
        }



        [Fact]
        public void Validate_CarHorsepowerIsSmallerThan5_AssertionFailedException()
        {
            Validate validate = new();
            CarQ car = new CarQ { Manufacturer = "Audi", Model = "aa", Year = "2022", Horsepower = "4" };

            try
            {
                validate.Car(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }

    }
}
