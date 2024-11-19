using AzubiWiki_AzureFunctions_Program.Backend.Core.Interfaces;
using AzubiWiki_AzureFunctions_Program.Backend.Core.Model;
using AzubiWiki_AzureFunctions_Program.Backend.Repositories;
using FluentAssertions.Execution;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.CarRepository.UnitTest
{
    public class Create
    {
        private readonly Mock<IFileHandler> _mockFileHandler = new Mock<IFileHandler>();


        [Fact]
        public async Task CarRepository_FileDoesNotExistValidCredentials_Success()
        {
            _mockFileHandler.Setup(c => c.Exists(It.IsAny<string>())).Returns(false);
            _mockFileHandler.Setup(c => c.Create(It.IsAny<string>())).Returns(It.IsAny<FileStream>());
            _mockFileHandler.Setup(c => c.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            Car car = new Car { Manufacturer = "Audi", Model = "A8", Horsepower = 123, ID = Guid.NewGuid(), Year = 2022 };

            Repositories.CarRepository repo = new(_mockFileHandler.Object);

            await repo.Create(car);

            _mockFileHandler.Verify(c => c.WriteAllTextAsync(It.IsAny<string>(),It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public void CarRepository_FileExistsCreateValidCredentials_Success()
        {
            _mockFileHandler.Setup(c => c.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(c => c.Create(It.IsAny<string>())).Returns(It.IsAny<FileStream>());
            _mockFileHandler.Setup(c => c.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<Task>());

            Car car = new Car { Manufacturer = "Audi", Model = "A8", Horsepower = 123, ID = Guid.NewGuid(), Year = 2022 };
            List<Car> cars = new List<Car>();
            cars.Add(car);
            string jsonContent = JsonConvert.SerializeObject(cars, Formatting.Indented);

            _mockFileHandler.Setup(c => c.ReadAllTextAsync(It.IsAny<string>())).Returns(Task.FromResult(jsonContent));

            Repositories.CarRepository repo = new(_mockFileHandler.Object);

            repo.Create(car);

            _mockFileHandler.Verify(c => c.Create(It.IsAny<string>()), Times.Never);
            _mockFileHandler.Verify(c => c.ReadAllTextAsync(It.IsAny<string>()), Times.Once);
            _mockFileHandler.Verify(c => c.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        
    }
}
