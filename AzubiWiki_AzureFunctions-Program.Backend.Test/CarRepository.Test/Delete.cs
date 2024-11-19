using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Newtonsoft.Json;

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.CarRepository.Test
{
    public class Delete
    {
        private readonly Mock<IFileHandler> _mockFileHandler = new Mock<IFileHandler>();



        [Fact]
        public async Task CarRepository_ValidCredentials_Successful()
        {
            _mockFileHandler.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(d => d.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            List<Car> cars = new();
            Car car = new Car { Horsepower = 123, ID = Guid.NewGuid(), Manufacturer = "AAA", Model = "1", Year =312 };
            cars.Add(car);
            
            car.ID = Guid.NewGuid();
            car.Model = "2";
            car.Manufacturer = "BBB";
            cars.Add(car);

            _mockFileHandler.Setup(d => d.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(cars)));

            Repositories.CarRepository repo = new(_mockFileHandler.Object);

            await repo.Delete(car.ID);

            _mockFileHandler.Verify(d => d.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task CarRepository_FileDoesNotExist_FileNotFoundException()
        {
            _mockFileHandler.Setup(d => d.Exists(It.IsAny<string>())).Returns(false);

            Repositories.CarRepository repo = new(_mockFileHandler.Object);

            try
            {
                await repo.Delete(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType<FileNotFoundException>();
            }
        }



        [Fact]
        public async Task CarRepository_ObjectNotFound_AssertionFailedException()
        {
            _mockFileHandler.Setup(d => d.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(d => d.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            List<Car> cars = new();
            Car car = new Car { Horsepower = 123, ID = Guid.NewGuid(), Manufacturer = "AAA", Model = "1", Year = 312 };
            cars.Add(car);

            car.ID = Guid.NewGuid();
            car.Model = "2";
            car.Manufacturer = "BBB";
            cars.Add(car);

            _mockFileHandler.Setup(d => d.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(JsonConvert.SerializeObject(cars)));

            Repositories.CarRepository repo = new(_mockFileHandler.Object);

            try
            {
                await repo.Delete(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType<AssertionFailedException>();
            }
        }
    }
}
