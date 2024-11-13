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

namespace AzubiWiki_AzureFunctions_Program.Backend.UnitTest.GarageRepository.Test
{
    public class RemoveCarFromGarage
    {
        private readonly Mock<IFileHandler> _mockFileHandler = new Mock<IFileHandler>();


        [Fact]
        public async Task GarageRepository_ValidCredentials_Successful()
        {
            _mockFileHandler.Setup(a => a.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(a => a.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            List<Car> cars = new List<Car>{ new Car { ID = Guid.NewGuid(), Horsepower = 123, Manufacturer = "audi", Model = "a", Year = 2000 } };
            List<Garage> garages = new List<Garage> { new Garage { BelongsTo = "me", ID = Guid.NewGuid(), Cars = new List<Car> { cars[0] } } };

            _mockFileHandler.Setup(a => a.ReadAllTextAsync("C:\\Users\\Ioannis.Batenkof\\source\\repos\\AzubiWiki_AzureFunctions-Program.Backend.Database\\File_GarageStorage.json")).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));
            _mockFileHandler.Setup(a => a.ReadAllTextAsync("C:\\Users\\Ioannis.Batenkof\\source\\repos\\AzubiWiki_AzureFunctions-Program.Backend.Database\\File_CarStorage.json")).Returns(Task.FromResult(JsonConvert.SerializeObject(cars, Formatting.Indented)));

            Repositories.GarageRepository repo = new(_mockFileHandler.Object);

            await repo.RemoveCarFromGarage(garages[0].Cars[0].ID, garages[0].ID);

            _mockFileHandler.Verify(a => a.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task GarageRepository_GarageNotFound_AssertionFailedException()
        {
            _mockFileHandler.Setup(a => a.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(a => a.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            List<Car> cars = new List<Car> { new Car { ID = Guid.NewGuid(), Horsepower = 123, Manufacturer = "audi", Model = "a", Year = 2000 } };
            List<Garage> garages = new List<Garage> { new Garage { BelongsTo = "me", ID = Guid.NewGuid(), Cars = new List<Car> { cars[0] } } };

            _mockFileHandler.Setup(a => a.ReadAllTextAsync("C:\\Users\\Ioannis.Batenkof\\source\\repos\\AzubiWiki_AzureFunctions-Program.Backend.Database\\File_GarageStorage.json")).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));
            _mockFileHandler.Setup(a => a.ReadAllTextAsync("C:\\Users\\Ioannis.Batenkof\\source\\repos\\AzubiWiki_AzureFunctions-Program.Backend.Database\\File_CarStorage.json")).Returns(Task.FromResult(JsonConvert.SerializeObject(cars, Formatting.Indented)));

            Repositories.GarageRepository repo = new(_mockFileHandler.Object);

            try
            {
                await repo.RemoveCarFromGarage(cars[0].ID, Guid.NewGuid());
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }


        [Fact]
        public async Task GarageRepository_CarNotFound_AssertionFailedException()
        {
            _mockFileHandler.Setup(a => a.Exists(It.IsAny<string>())).Returns(true);
            _mockFileHandler.Setup(a => a.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            List<Car> cars = new List<Car> { new Car { ID = Guid.NewGuid(), Horsepower = 123, Manufacturer = "audi", Model = "a", Year = 2000 } };
            List<Garage> garages = new List<Garage> { new Garage { BelongsTo = "me", ID = Guid.NewGuid(), Cars = new List<Car> { cars[0] } } };

            _mockFileHandler.Setup(a => a.ReadAllTextAsync("C:\\Users\\Ioannis.Batenkof\\source\\repos\\AzubiWiki_AzureFunctions-Program.Backend.Database\\File_GarageStorage.json")).Returns(Task.FromResult(JsonConvert.SerializeObject(garages, Formatting.Indented)));
            _mockFileHandler.Setup(a => a.ReadAllTextAsync("C:\\Users\\Ioannis.Batenkof\\source\\repos\\AzubiWiki_AzureFunctions-Program.Backend.Database\\File_CarStorage.json")).Returns(Task.FromResult(JsonConvert.SerializeObject(cars, Formatting.Indented)));

            Repositories.GarageRepository repo = new(_mockFileHandler.Object);

            try
            {
                await repo.RemoveCarFromGarage( Guid.NewGuid(), garages[0].ID);
            }
            catch (Exception ex)
            {
                Assert.IsType<XunitException>(ex);
            }
        }



        [Fact]
        public async Task GarageRepository_FileDoesNotExist_FileNotFoundException()
        {
            _mockFileHandler.Setup(a => a.Exists(It.IsAny<string>())).Returns(false);

            Repositories.GarageRepository repo = new(_mockFileHandler.Object);

            try
            {
                await repo.RemoveCarFromGarage(Guid.NewGuid(), Guid.NewGuid());
            }
            catch (Exception ex)
            {
                Assert.IsType<FileNotFoundException>(ex);
            }
        }


    }
}
