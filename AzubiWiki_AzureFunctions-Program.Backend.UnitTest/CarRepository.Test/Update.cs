using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.CarRepository.Test
{
    public class Update
    {
        private readonly Mock<IFileHandler> _mockFileHandler = new Mock<IFileHandler>();


        [Fact]
        public async Task CarRepository_ValidCredentials_Success()
        {
            _mockFileHandler.Setup(u => u.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(c => c.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            Car car = new Car { Horsepower = 123, ID = Guid.NewGuid(), Manufacturer = "Audi", Model = "1", Year = 2000 };
            List<Car> cars = new List<Car>();
            cars.Add(car);

            _mockFileHandler.Setup(u => u.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(cars, Formatting.Indented)));

            car.Model = "2";
            car.Horsepower = 321;
            car.Year = 2022;

            Repositories.CarRepository repo = new(_mockFileHandler.Object);

            await repo.Update(car);

            _mockFileHandler.Verify(u => u.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }



        [Fact]
        public async Task CarRepository_ObjectNotFound_AssertionFailedException()
        {
            _mockFileHandler.Setup(u => u.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(c => c.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            Car car = new Car { Horsepower = 123, ID = Guid.NewGuid(), Manufacturer = "Audi", Model = "1", Year = 2000 };
            List<Car> cars = new List<Car>();
            cars.Add(car);

            _mockFileHandler.Setup(u => u.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(cars, Formatting.Indented)));

            car.Model = "2";
            car.Horsepower = 321;
            car.Year = 2022;
            car.ID = Guid.NewGuid();

            Repositories.CarRepository repo = new(_mockFileHandler.Object);

            try
            {
                await repo.Update(car);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }



        [Fact]
        public async Task CarRepository_FileDoesNotExist_FileNotFoundException()
        {
            _mockFileHandler.Setup(u => u.Exists(It.IsAny<string>())).Returns(false);

            Repositories.CarRepository repo = new(_mockFileHandler.Object);

            try
            {
                await repo.Update(new Car());
            }
            catch (Exception ex)
            {
                Assert.IsType<FileNotFoundException>(ex);
            }
        }
    }
}
