using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.CarRepository.UnitTest
{
    public class Read
    {
        private readonly Mock<IFileHandler> _fileHandlerMock = new Mock<IFileHandler>();


        [Fact]
        public async Task CarRepository_ObjectFound_Successful()
        {
            List<Car> cars = new List<Car>();
            Car car = new Car { Manufacturer = "Audi", Horsepower = 123, ID = Guid.NewGuid(), Model = "1", Year = 2002 };
            cars.Add(car);
            car = new Car { Manufacturer = "BMW", Horsepower = 312, Year = 2004, Model = "2", ID = Guid.NewGuid() };
            cars.Add(car);

            string jsonContent = JsonConvert.SerializeObject(cars);

            _fileHandlerMock.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);
            _fileHandlerMock.Setup(r => r.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(jsonContent));

            Repositories.CarRepository repo = new(_fileHandlerMock.Object);

            Car gar = await repo.Read(car.ID);

            Assert.Equal("2", gar.Model);
        }



        [Fact]
        public async Task CarRepository_ObjectNotFound_AssertionFailedException()
        {
            List<Car> cars = new List<Car>();
            Car car = new Car { Manufacturer = "Audi", Horsepower = 123, ID = Guid.NewGuid(), Model = "1", Year = 2002 };
            cars.Add(car);
            car = new Car { Manufacturer = "BMW", Horsepower = 312, Year = 2004, Model = "2", ID = Guid.NewGuid() };
            cars.Add(car);

            string jsonContent = JsonConvert.SerializeObject(cars);

            _fileHandlerMock.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);
            _fileHandlerMock.Setup(r => r.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(jsonContent));

            Repositories.CarRepository repo = new(_fileHandlerMock.Object);

            try
            {
                await repo.Read(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }



        [Fact]
        public async Task CarRepository_FileDoesNotExist_FileNotFoundException()
        {
            _fileHandlerMock.Setup(r => r.Exists(It.IsAny<string>())).Returns(false);

            Repositories.CarRepository repo = new(_fileHandlerMock.Object);

            try
            {
                await repo.Read(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                Assert.IsType<FileNotFoundException>(ex);
            }
        }
    }
}
